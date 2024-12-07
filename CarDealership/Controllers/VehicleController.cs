﻿using CarDealership.Services.Data.Interfaces;
using CarDealership.Web.Infrastructure;
using CarDealership.Web.ViewModels.Vehicle;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CarDealership.Web.Controllers
{
    public class VehicleController : BaseController
    {
        private readonly IVehicleService vehicleService;

        public VehicleController(IVehicleService _vehicleService, IManagerService managerService)
            : base(managerService)
        {
            vehicleService = _vehicleService;
        }


        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> Index([FromQuery] VehicleAllViewModel query)
        {


            var result = await vehicleService.GetAllAsync(query.Make, query.Model, query.Type,
                query.LocatedIn, query.Category, query.Search, query.Sorting, query.CurrentPage, VehicleAllViewModel.VehicleOnPage);


            query.VehiclesCount = result.TotalVehicles;
            query.Makes = await vehicleService.AllMakeNames();
            query.Models = await vehicleService.AllModelsNames();
            query.Types = await vehicleService.AllTypesNames();
            query.Categories = await vehicleService.AllCategoriesNames();
            query.LocatedIns = await vehicleService.AllLocations();
            query.Vehicles = result.Vehicles;

            return View(query);


        }

        [HttpGet]
        public async Task<IActionResult> Add(VehicleViewModel vehicle, Guid id)
        {
            bool isManager = await this.managerService.ExistById(User.GetUserId());
            if (!isManager)
            {
                return this.RedirectToAction(nameof(Index));
            }

            Guid vehicleGuid = Guid.Empty;
            bool isIdValid = this.IsGuidValid(id.ToString(), ref vehicleGuid);
            if (!isIdValid)
            {
                return this.RedirectToAction(nameof(Index));
            }

            var model = new VehicleViewModel()
            {
                Make = vehicle.Make,
                Model = vehicle.Model,
                FuelType = vehicle.FuelType,
                Gearbox = vehicle.Gearbox,
                Year = vehicle.Year,
                Seats = vehicle.Seats,
                Doors = vehicle.Doors,
                TankCapacity = vehicle.TankCapacity,
                Horsepower = vehicle.Horsepower,
                Cubage = vehicle.Cubage,
                Price = vehicle.Price.Scale,
                ImageUrl = vehicle.ImageUrl,
                CategoryId = vehicle.CategoryId,
                TypeId = vehicle.TypeId,
                VehicleTypes = await vehicleService.AllTypes(),
                VehicleCategories = await vehicleService.AllCategories()

            };

            return View(model);
        }


        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Add(VehicleViewModel model)
        {

            if (!this.ModelState.IsValid)
            {
                return this.RedirectToAction(nameof(Index));
            }
            model.VehicleCategories = await vehicleService.AllCategories();
            model.VehicleTypes = await vehicleService.AllTypes();

            await this.vehicleService.AddVehicleAsync(model);

            return this.RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Edit(Guid id)
        {
            bool isManager = await this.managerService.ExistById(User.GetUserId());
            if (!isManager)
            {
                return this.RedirectToAction(nameof(Index));
            }

            Guid vehicleGuid = Guid.Empty;
            bool isIdValid = this.IsGuidValid(id.ToString(), ref vehicleGuid);
            if (!isIdValid)
            {
                return this.RedirectToAction(nameof(Index));
            }

            var vehicle = await vehicleService.VehicleSpecificationsById(id);

            var model = new VehicleViewModel()
            {
                Make = vehicle.Make,
                Model = vehicle.Model,
                FuelType = vehicle.FuelType,
                Gearbox = vehicle.Gearbox,
                Year = vehicle.Year,
                Seats = vehicle.Seats,
                Doors = vehicle.Doors,
                TankCapacity = vehicle.TankCapacity,
                Horsepower = vehicle.Horsepower,
                Cubage = vehicle.Cubage,
                Price = vehicle.Price.Scale,
                ImageUrl = vehicle.ImageUrl,
                CategoryId = vehicle.Category.Id,
                TypeId = vehicle.Type.Id,
                VehicleTypes = await vehicleService.AllTypes(),
                VehicleCategories = await vehicleService.AllCategories()

            };

            return View(model);
        }

        [HttpPost]

        public async Task<IActionResult> Edit(VehicleViewModel model)
        {
            bool isManager = await this.managerService.ExistById(User.GetUserId());
            if (!isManager)
            {
                ModelState.AddModelError(string.Empty, "Unexpected error. Please contact administrator");
                return RedirectToAction(nameof(Index));
            }

            if (!ModelState.IsValid)
            {
                return View(model);
            }


            await vehicleService.EditVehicleAsync(model);


            return RedirectToAction(nameof(Specifications), "Vehicle", new { id = model.Id });
        }

        [AllowAnonymous]
        public async Task<IActionResult> Specifications(string? id)
        {
            Guid vehicleGuid = Guid.Empty;

            bool isGuidValid = this.IsGuidValid(id, ref vehicleGuid);

            if (!isGuidValid)
            {
                return this.RedirectToAction(nameof(Index));
            }

            var vehicle = await this.vehicleService
                .VehicleSpecificationsById(vehicleGuid);

            if (vehicle == null)
            {
                return this.RedirectToAction(nameof(Index));
            }

            return this.View(vehicle);
        }

        [HttpGet]
        public async Task<IActionResult> Delete(string? id)
        {
            bool isManager = await this.managerService.ExistById(User.GetUserId());
            if (!isManager)
            {
                return RedirectToAction(nameof(Index));
            }

            Guid vehicleGuid = Guid.Empty;

            bool isGuidValid = this.IsGuidValid(id, ref vehicleGuid);

            if (!isGuidValid)
            {
                return RedirectToAction(nameof(Index));
            }

            DeleteViewModel deleteModel = await vehicleService.DeleteByIdAsync(vehicleGuid);

            if (deleteModel == null)
            {
                return RedirectToAction(nameof(Index));
            }

            return View(deleteModel);
        }

        [HttpPost]
        public async Task<IActionResult> SoftDelete(DeleteViewModel delete)
        {
            bool isManager = await this.managerService.ExistById(User.GetUserId());
            if (!isManager)
            {
                return RedirectToAction(nameof(Index));
            }

            Guid vehicleGuid = Guid.Empty;

            if (!this.IsGuidValid(delete.Id, ref vehicleGuid))
            {
                return RedirectToAction(nameof(Index));
            }

            bool isDeleted = await vehicleService
                .SoftDeleteAsync(vehicleGuid);

            if (!isDeleted)
            {
                return RedirectToAction(nameof(Delete), new { id = delete.Id });
            }

            return RedirectToAction(nameof(Index));
        }
    }
}
