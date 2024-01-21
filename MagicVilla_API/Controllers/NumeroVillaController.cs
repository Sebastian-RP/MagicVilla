using AutoMapper;
using MagicVilla_API.Datos;
using MagicVilla_API.Modelos;
using MagicVilla_API.Modelos.Dto;
using MagicVilla_API.Repositorio.IRepositorio;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Data;
using System.Net;

namespace MagicVilla_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class NumeroVillaController : ControllerBase
    {
        private readonly ILogger<NumeroVillaController> _logger;
        private readonly IVillaRepositorio _villaRepo;
        private readonly INumeroVillaRepositorio _numeroRepo;
        private readonly IMapper _mapper;
        protected APIResponse _response;

        public NumeroVillaController(
            ILogger<NumeroVillaController> logger,
            IVillaRepositorio villaRepo,
            INumeroVillaRepositorio numeroRepo,
            IMapper mapper
            ) 
        { 
            _logger = logger;
            _villaRepo = villaRepo;
            _numeroRepo = numeroRepo;
            _mapper = mapper;
            _response = new();
        }


        [HttpGet]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<APIResponse>> GetNumeroVillas()
        {
            try
            {
                _logger.LogInformation("Obtener Numeros Villas");

                IEnumerable<NumeroVilla> NumeroVillaList = await _numeroRepo.ObtenerTodos(incluirPropiedades: "Villa");

                _response.Resultado = _mapper.Map<IEnumerable<NumeroVillaDto>>(NumeroVillaList);
                _response.statusCode = HttpStatusCode.OK;

                return Ok(_response);
            }
            catch (Exception ex)
            {
                _response.IsExitoso = false;
                _response.ErrorMessage = new List<string>() { ex.ToString() };
            }
            return _response;
        }

        [HttpGet("{id:int}", Name = "GetNumeroVilla")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<APIResponse>> GetNumeroVilla(int id) 
        {
            try
            {
                if (id == 0)
                {
                    _logger.LogError("Error al traer Numero Villa con Id" + id);
                    _response.statusCode = HttpStatusCode.BadRequest;
                    return BadRequest(_response);
                }

                //var villa = VillaStore.villaList.FirstOrDefault(v => v.Id == id);
                var numeroVilla = await _numeroRepo.Obtener(v => v.VillaNo == id, incluirPropiedades: "Villa"); 

                if (numeroVilla == null)
                {
                    _response.statusCode = HttpStatusCode.NotFound;
                    _response.IsExitoso = false;
                    return NotFound();
                }

                _response.Resultado = _mapper.Map<NumeroVillaDto>(numeroVilla);
                _response.statusCode = HttpStatusCode.OK;

                return Ok(_response);
            }
            catch (Exception ex)
            {
                _response.IsExitoso = false;
                _response.ErrorMessage = new List<string>() { ex.ToString() };
            }
            return _response;
        }

        [HttpPost]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<APIResponse>> CrearNumeroVilla([FromBody] NumeroVillaCreateDto crearDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(crearDto);
                }

                //Validacion personalizada ModelState
                //if (VillaStore.villaList.FirstOrDefault(villa => villa.Nombre.ToLower() == villaDto.Nombre.ToLower()) != null)
                if (await _numeroRepo.Obtener(villa => villa.VillaNo == crearDto.VillaNo) != null)
                {
                    ModelState.AddModelError("ErrorMessage", "El numero de Villa ya existe");
                    return BadRequest(ModelState);
                }

                if (await _villaRepo.Obtener(v => v.Id == crearDto.VillaId) == null)
                {
                    ModelState.AddModelError("ErrorMessage", "El Id de la villa no existe!");
                    return BadRequest(ModelState);
                }

                if (crearDto == null)
                {
                    return BadRequest();
                }

                //villaDto.Id = VillaStore.villaList.OrderByDescending(v => v.Id).FirstOrDefault().Id + 1;
                //VillaStore.villaList.Add(villaDto);

                NumeroVilla modelo = _mapper.Map<NumeroVilla>(crearDto); //este codigo remplaza el modelo que está escrito abajo

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

                modelo.FechaCreacion = DateTime.Now;
                modelo.FechaActualizacion = DateTime.Now;
                await _numeroRepo.Crear(modelo);
                _response.Resultado = modelo;
                _response.statusCode = HttpStatusCode.Created;
                
                return CreatedAtRoute("GetNumeroVilla", new { id = modelo.VillaNo }, _response);
            }
            catch (Exception ex)
            {
                _response.IsExitoso = false;
                _response.ErrorMessage = new List<string> { ex.ToString() };
            }
            return _response;
        }

        [HttpDelete("{id:int}")]
        [Authorize(Roles = "admin")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteNumeroVilla(int id)
        {
            try
            {
                if (id == 0)
                {
                    _response.IsExitoso = false;
                    _response.statusCode = HttpStatusCode.BadRequest;
                    return BadRequest();
                }
                //var villa = VillaStore.villaList.FirstOrDefault(v => v.Id == id);
                var numeroVilla = await _numeroRepo.Obtener(v => v.VillaNo == id);

                if (numeroVilla == null)
                {
                    _response.IsExitoso = false;
                    _response.statusCode = HttpStatusCode.NotFound;
                    return NotFound();
                }

                //VillaStore.villaList.Remove(villa);
                await _numeroRepo.Remover(numeroVilla);
                _response.statusCode = HttpStatusCode.NoContent;

                return Ok(_response);
            }
            catch(Exception ex)
            {
                _response.IsExitoso = false;
                _response.ErrorMessage = new List<string>() { ex.ToString() };
            }
            return BadRequest(_response);
        }

        [HttpPut("{id:int}")]
        [Authorize(Roles = "admin")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> UpdateNumeroVilla(int id, [FromBody] NumeroVillaUpdateDto updateDto)
        {
            if (updateDto == null || id != updateDto.VillaNo)
            {
                _response.IsExitoso = false;
                _response.statusCode = HttpStatusCode.BadRequest;
                return BadRequest();
            }

            if (await _villaRepo.Obtener(v => v.Id == updateDto.VillaId) == null)
            {
                ModelState.AddModelError("ErrorMessage", "El Id de la villa no existe!");
                return BadRequest(ModelState);
            }
            //var villa = VillaStore.villaList.FirstOrDefault(v => v.Id == id);
            //villa.Nombre = villaDto.Nombre;
            //villa.Ocupantes = villaDto.Ocupantes;
            //villa.MetrosCuadrados = villaDto.MetrosCuadrados;

            NumeroVilla modelo = _mapper.Map<NumeroVilla>(updateDto);

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

            await _numeroRepo.Actualizar(modelo);
            _response.statusCode = HttpStatusCode.NoContent;

            return Ok(_response);
        }
    }
}
