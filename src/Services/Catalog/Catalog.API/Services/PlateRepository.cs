using Catalog.API.Data;
using Catalog.API.Helpers;
using LinqKit;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query.SqlExpressions;
using Plates.Shared;

namespace Catalog.API.Services;

public class PlateRepository : IPlateRepository
{
    private readonly ApplicationDbContext _context;
    public PlateRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<PagedResult<Plate>> GetAsync(int pageNum = 1, int pageSize = 20)
    {
        var items = await _context.Plates
            .Skip((pageNum - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        var pagedResults = new PagedResult<Plate>
        {
            Items = items,
            TotalCount = _context.Plates.Count(),
            PageNumber = pageNum,
            PageSize = pageSize
        };

        return pagedResults;
    }

    public async Task<PagedResult<Plate>> GetAsync(int pageNum = 1,
                                                   int pageSize = 20,
                                                   string sortColumn = "Registration",
                                                   string sortDirection = "asc",
                                                   string filter = null)
    {
        var query = GetQuery(filter);   //initial query - choose appropriate column based on filter contents

        switch (sortColumn.ToLower())   //add the orderby option
        {
            case "saleprice":
                query = sortDirection == "asc" ? query.OrderBy(p => p.SalePrice) : query.OrderByDescending(p => p.SalePrice);
                break;
            default:
                query = sortDirection == "asc" ? query.OrderBy(p => p.Registration) : query.OrderByDescending(p => p.Registration);
                break;
        }

        var items = await query
            .Skip((pageNum - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        var pagedResults = new PagedResult<Plate>
        {
            Items = items,
            TotalCount = await query.CountAsync(),
            PageNumber = pageNum,
            PageSize = pageSize,
            SortColumn = sortColumn,
            SortDirection = sortDirection,
            Filter = filter
        };

        return pagedResults;
    }

    public async Task<bool> ExistsAsync(string registration)
    {
        return await _context.Plates.AnyAsync(p => p.Registration == registration);
    }

    public async Task CreateAsync(Plate plate)
    {
        await _context.Plates.AddAsync(plate);
        await _context.SaveChangesAsync();
    }

    /// <summary>
    /// return a query based on filter
    ///  - empty filter -> all 
    ///  - numbers only -> filter on Numbers column (exact match)
    ///  - length < 4   -> filter on Letters column (start with)
    ///  - otherwise    -> filter on Registration (like)
    /// </summary>
    /// <param name="filter"></param>
    /// <returns></returns>
    private IQueryable<Plate> GetQuery(string filter)
    {
        IQueryable<Plate> query = _context.Plates;

        // no filter
        if (string.IsNullOrWhiteSpace(filter))
            return query;

        filter = filter.Replace(" ", "").ToUpper();

        var isNumber = int.TryParse(filter, out int filterNumber);

        //filter is just numbers
        if (isNumber)
            return query.Where(p => p.Numbers == filterNumber);

        //filter simple match on reg letters - assuming filter is just the user's initials
        if (filter.Length < 4)        
            return query.Where(p => p.Letters.StartsWith(filter));


        //otherwise assume user has entered their name - find plates that look like variations on their name.
        //NOTE: this is not an ideal solution - not practical for a db with millions of records.
        //
        //.Like(p.Registration, $"%{c}%") - this doesn't work out-the-box, so use a nuget LinqKit predicate builder
        //return query.Where(p => combinations.Any(c => EF.Functions.Like(p.Registration, $"%{c}%")));

        var predicate = GetPredicateForRegistration(filter);
        query = query.AsExpandable().Where(predicate);
        return query;
    }

    /// <summary>
    /// return a dynamic predicate based on variations of the filter.
    /// predicate is a series of OR conditions for each filter variation matching using the sql LIKE operation.
    /// this can then be used in the WHERE clause of a query
    /// </summary>
    /// <param name="filter"></param>
    /// <returns></returns>
    private static ExpressionStarter<Plate> GetPredicateForRegistration(string filter)
    {
        var combinations = PlateHelper.GetCombinations(filter);     //get variations of filter (reg)

        var predicate = PredicateBuilder.New<Plate>(false);         //start with false

        foreach (var c in combinations)                             //construct "OR" for each variation
        {
            var pattern = $"%{c}%";
            predicate = predicate.Or(p => EF.Functions.Like(p.Registration, pattern));
        }

        return predicate;

    }

}
