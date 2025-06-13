using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Plates.Shared;
using WebMVC.Services;
using WebMVC.ViewModels;

namespace WebMVC.Controllers
{
    public class PlatesController : Controller
    {
        private readonly IPlateService _plateService;

        public PlatesController(IPlateService plateService)
        {
            _plateService = plateService;
        }
        // GET: PlatesController
        [HttpGet]
        public async Task<ActionResult> Index(int page = 1, 
                                              int pageSize = 20,
                                              string sortColumn = "Registration", 
                                              string sortDirection = "asc",
                                              string searchString = "")
        {
            var vm = await _plateService.GetPlates(page, pageSize, sortColumn, sortDirection, searchString);

            return View(vm);  // pass the list to the view
        }

        // GET: PlatesController/Details/5
        [HttpGet]
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: PlatesController/Create
        [HttpGet]
        public ActionResult Create()
        {
            var plate = new PlateDTO();
            return View(plate);
        }

        // POST: PlatesController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(PlateDTO model)
        {
            try
            {
                //validation tbd
                var success = await _plateService.Create(model);

                if (!success)
                {
                    return View(model);
                }

                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View(model);
            }
        }


        #region auto-generated code - tbd

        // GET: PlatesController/Edit/5
        [HttpGet]
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: PlatesController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: PlatesController/Delete/5
        [HttpGet]
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: PlatesController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }
        #endregion
    }
}
