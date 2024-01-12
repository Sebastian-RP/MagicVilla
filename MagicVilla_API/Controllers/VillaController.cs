﻿using AutoMapper;
using MagicVilla_API.Datos;
using MagicVilla_API.Modelos;
using MagicVilla_API.Modelos.Dto;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace MagicVilla_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class VillaController : ControllerBase
    {
        private readonly ILogger<VillaController> _logger;
        private readonly ApplicationDbContext _db;
        private readonly IMapper _mapper;

        public VillaController(
            ILogger<VillaController> logger,
            ApplicationDbContext context,
            IMapper mapper
            ) 
        { 
            _logger = logger;
            _db = context;
            _mapper = mapper;
        }


        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<IEnumerable<VillaDto>>> Getvillas()
        {
            _logger.LogInformation("Obtener las Villas");

            IEnumerable<Villa> villaList = await _db.Villas.ToListAsync();
            return Ok(_mapper.Map<IEnumerable<VillaDto>>(villaList));
        }

        [HttpGet("{id:int}", Name = "GetVilla")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<VillaDto>> GetVilla(int id) 
        {
            if(id == 0 )
            {
                _logger.LogError("Error al traer Villa con Id" + id);
                return BadRequest();
            }

            //var villa = VillaStore.villaList.FirstOrDefault(v => v.Id == id);
            var villa = await _db.Villas.FirstOrDefaultAsync(v => v.Id == id);

            if (villa == null)
            {
                return NotFound();
            }

            return Ok(_mapper.Map<VillaDto>(villa));
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<VillaDto>> CrearVilla([FromBody] VillaCreateDto crearDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(crearDto);
            }

            //Validacion personalizada ModelState
            //if (VillaStore.villaList.FirstOrDefault(villa => villa.Nombre.ToLower() == villaDto.Nombre.ToLower()) != null)
            if (await _db.Villas.FirstOrDefaultAsync(villa => villa.Nombre.ToLower() == crearDto.Nombre.ToLower()) != null)
            {
                ModelState.AddModelError("Nombre Existe", "La Villa con ese nombre ya existe");
                return BadRequest(ModelState);
            }

            if (crearDto == null)
            {
                return BadRequest();
            }

            //villaDto.Id = VillaStore.villaList.OrderByDescending(v => v.Id).FirstOrDefault().Id + 1;
            //VillaStore.villaList.Add(villaDto);

            Villa modelo = _mapper.Map<Villa>(crearDto); //este codigo remplaza el modelo que está escrito abajo

            //Villa modelo = new()
            //{
            //    Nombre = crearDto.Nombre,
            //    Detalle = crearDto.Detalle,
            //    ImagenUrl = crearDto.ImagenUrl,
            //    Ocupantes = crearDto.Ocupantes,
            //    Tarifa = crearDto.Tarifa,
            //    MetrosCuadrados = crearDto.MetrosCuadrados,
            //    Amenidad = crearDto.Amenidad
            //};

            await _db.Villas.AddAsync(modelo);
            await _db.SaveChangesAsync();

            return CreatedAtRoute("GetVilla", new { id = modelo.Id }, modelo);
        }

        [HttpDelete("{id:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteVilla(int id)
        {
            if (id==0)
            {
                return BadRequest();
            }
            //var villa = VillaStore.villaList.FirstOrDefault(v => v.Id == id);
            var villa = await _db.Villas.FirstOrDefaultAsync(v => v.Id == id);

            if (villa == null)
            {
                return NotFound();
            }

            //VillaStore.villaList.Remove(villa);
            _db.Villas.Remove(villa);
            await _db.SaveChangesAsync();

            return NoContent();
        }

        [HttpPut("{id:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> UpdateVilla(int id, [FromBody] VillaUpdateDto updateDto)
        {
            if (updateDto == null || id != updateDto.Id)
            {
                return BadRequest();
            }
            //var villa = VillaStore.villaList.FirstOrDefault(v => v.Id == id);
            //villa.Nombre = villaDto.Nombre;
            //villa.Ocupantes = villaDto.Ocupantes;
            //villa.MetrosCuadrados = villaDto.MetrosCuadrados;

            Villa modelo = _mapper.Map<Villa>(updateDto);

            //Villa modelo = new()
            //{
            //    Id = updateDto.Id,
            //    Nombre = updateDto.Nombre,
            //    Detalle = updateDto.Detalle,
            //    ImagenUrl = updateDto.ImagenUrl,
            //    Ocupantes = updateDto.Ocupantes,
            //    Tarifa = updateDto.Tarifa,
            //    MetrosCuadrados = updateDto.MetrosCuadrados,
            //    Amenidad = updateDto.Amenidad
            //};

            _db.Update(modelo);
            await _db.SaveChangesAsync();

            return NoContent();
        }

        [HttpPatch("{id:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> UpdatePartialVilla(int id, JsonPatchDocument<VillaUpdateDto> patchDto)
        {
            if (patchDto == null || id == 0)
            {
                return BadRequest();
            }
            //var villa = VillaStore.villaList.FirstOrDefault(v => v.Id == id);
            var villa = await _db.Villas.AsNoTracking().FirstOrDefaultAsync(v => v.Id == id);

            VillaUpdateDto villaDto = _mapper.Map<VillaUpdateDto>(villa);

            //VillaUpdateDto villaDto = new()
            //{
            //    Id = villa.Id,
            //    Nombre = villa.Nombre,
            //    Detalle = villa.Detalle,
            //    ImagenUrl = villa.ImagenUrl,
            //    Ocupantes = villa.Ocupantes,
            //    Tarifa = villa.Tarifa,
            //    MetrosCuadrados = villa.MetrosCuadrados,
            //    Amenidad = villa.Amenidad
            //};

            if (villa == null) { return BadRequest(); }

            //patchDto.ApplyTo(villa, ModelState);
            patchDto.ApplyTo(villaDto, ModelState);

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            Villa modelo = _mapper.Map<Villa>(villaDto);

            //Villa modelo = new()
            //{
            //    Id = villaDto.Id,
            //    Nombre = villaDto.Nombre,
            //    Detalle = villaDto.Detalle,
            //    ImagenUrl = villaDto.ImagenUrl,
            //    Ocupantes = villaDto.Ocupantes,
            //    Tarifa = villaDto.Tarifa,
            //    MetrosCuadrados = villaDto.MetrosCuadrados,
            //    Amenidad = villaDto.Amenidad
            //};

            _db.Villas.Update(modelo);
            await _db.SaveChangesAsync();

            return NoContent();
        }
    }
}
