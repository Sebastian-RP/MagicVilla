﻿using AutoMapper;
using MagicVilla_Web.Models;
using MagicVilla_Web.Models.Dto;
using MagicVilla_Web.Services.IServices;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Newtonsoft.Json;

namespace MagicVilla_Web.Controllers
{
    public class VillaController : Controller
    {
        private readonly IVillaService _villaService;
        private readonly IMapper _mapper;

        public VillaController(IVillaService villaService, IMapper mapper)
        {
            _villaService = villaService;
            _mapper = mapper;
        }

        public async Task<IActionResult> IndexVilla()
        {
            List<VillaDto> villaList = new();

            var response = await _villaService.ObtenerTodos<APIResponse>();

            if (response != null && response.IsExitoso)
            {
                villaList = JsonConvert.DeserializeObject<List<VillaDto>>(Convert.ToString(response.Resultado));
            }

            return View(villaList);
        }

        //asume que es Get
        public async Task<IActionResult> CrearVilla()
        {

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CrearVilla(VillaCreateDto modelo)
        {
            if (ModelState.IsValid)
            {
                var response = await _villaService.Crear<APIResponse>(modelo);
                if (response != null && response.IsExitoso)
                {
                    TempData["exitoso"] = "Villa Creada Exitosamente";
                    return RedirectToAction(nameof(IndexVilla));
                }
            }
            return View(modelo);
        }

        public async Task<IActionResult> ActualizarVilla(int VillaId)
        {
            var response = await _villaService.Obtener<APIResponse>(VillaId);

            if (response != null && response.IsExitoso)
            {
                VillaDto model = JsonConvert.DeserializeObject<VillaDto>(Convert.ToString(response.Resultado));
                return View(_mapper.Map<VillaUpdateDto>(model));
            }

            return NotFound();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ActualizarVilla(VillaUpdateDto modelo)
        {
            if (ModelState.IsValid)
            {
                var response = await _villaService.Actualizar<APIResponse>(modelo);

                if (response != null && response.IsExitoso)
                {
                    TempData["exitoso"] = "Villa Actualizada Exitosamente";
                    return RedirectToAction(nameof(IndexVilla));
                }
            }
            return View(modelo);
        }

        public async Task<IActionResult> RemoverVilla(int VillaId)
        {
            var response = await _villaService.Obtener<APIResponse>(VillaId);

            if (response != null && response.IsExitoso)
            {
                VillaDto model = JsonConvert.DeserializeObject<VillaDto>(Convert.ToString(response.Resultado));
                return View(model);
            }

            return NotFound();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RemoverVilla(VillaUpdateDto modelo)
        {
            var response = await _villaService.Remover<APIResponse>(modelo.Id);

            if (response != null && response.IsExitoso)
            {
                TempData["exitoso"] = "Villa Eliminada Exitosamente";
                return RedirectToAction(nameof(IndexVilla));
            }

            TempData["error"] = "Ocurrio un error al Remover";
            return View(modelo);
        }
    }
}
