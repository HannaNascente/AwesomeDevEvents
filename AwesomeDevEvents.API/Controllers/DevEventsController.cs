using AutoMapper;
using AwesomeDevEvents.API.Entities;
using AwesomeDevEvents.API.Models;
using AwesomeDevEvents.API.Persistence;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AwesomeDevEvents.API.Controllers
{
    [Route("api/dev-events")]
    [ApiController]
    public class DevEventsController : ControllerBase
    {
        private readonly DevEventsDbContext _context;
        private readonly IMapper _mapper;
        public DevEventsController(
            DevEventsDbContext context,
            IMapper mapper)
        {
            _context = context;
            _mapper = mapper; 
        }

        /// <summary>
        /// Obter todos os eventos
        /// </summary>
        /// <returns>Coleção de Eventos</returns>
        /// <response code="200">Success</response>    
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public IActionResult GetAll()
        {         
            var devEvents = _context.DevEvents
                .Include(de => de.Speakers);

            var viewModel = _mapper.Map<List<DevEventViewModel>>(devEvents);

            return Ok(viewModel);
        }

        /// <summary>
        /// Obter um evento
        /// </summary>
        /// <param name="id">Identificador do Evento</param>
        /// <returns>Dados do evento</returns>
        /// <response code="200">Success</response>
        /// <response code="204">No Content</response>
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public IActionResult GetById(Guid id)
        {
            var devEvent = _context.DevEvents
                .Include(de => de.Speakers)
                .SingleOrDefault(d => d.Id == id);

            if (devEvent == null)
            {
                return NoContent();
            }

            var viewModel = _mapper.Map<DevEventViewModel>(devEvent);

            return Ok(viewModel);
        }

        /// <summary>
        /// Cadastrar um evento
        /// </summary>
        /// <remarks>
        /// {"title":"string","description":"string","startDate":"2024-06-27T17:59:14.141Z","endDate":"2024-06-27T17:59:14.141Z"}
        /// </remarks>
        /// <param name="input">Dados do Evento</param>
        /// <returns>Objeto recém-criado</returns>
        /// <response code="201">Created</response>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        public IActionResult Post(DevEventInputModel input)
        {
            var devEvent = _mapper.Map<DevEvent>(input);

            _context.DevEvents.Add(devEvent);
            _context.SaveChanges();

            return CreatedAtAction(nameof(GetById), new { id = devEvent.Id }, devEvent);
        }

        /// <summary>
        /// Atualizar um evento
        /// </summary>
        /// <remarks>
        /// {"title":"string","description":"string","startDate":"2024-06-27T17:59:14.141Z","endDate":"2024-06-27T17:59:14.141Z"}
        /// </remarks>
        /// <param name="id">Identificador do Evento</param>
        /// <param name="input">Dados do Evento</param>
        /// <returns>Nada.</returns>
        /// <response code="200">Success</response>
        /// <response code="204">No Content</response>
        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public IActionResult Update(Guid id, DevEventInputModel input)
        {
            var devEvent = _context.DevEvents.SingleOrDefault(d => d.Id == id);

            if (devEvent == null)
            {
                return NoContent();
            }

            devEvent.Update(input.Title, input.Description, input.StartDate, input.EndDate);

            _context.DevEvents.Update(devEvent);
            _context.SaveChanges();

            return Ok();
        }

        /// <summary>
        /// Deletar um evento
        /// </summary>
        /// <param name="id">Identificador do Evento</param>
        /// <returns>Nada.</returns>
        /// <response code="200">Success</response>
        /// <response code="204">No Content</response>
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType (StatusCodes.Status204NoContent)]
        public IActionResult Delete(Guid id)
        {
            var devEvent = _context.DevEvents.SingleOrDefault(d => d.Id == id);

            if (devEvent == null)
            {
                return NoContent();
            }

            _context.DevEvents.Remove(devEvent);

            _context.SaveChanges();

            return Ok();
        }

        /// <summary>
        /// Cadastrar um Palestrante
        /// </summary>
        /// <remarks>
        /// {"name":"string","talkTitle":"string","talkDescription":"string","linkedInProfile":"string"}
        /// </remarks>
        /// <param name="id">Identificador do Evento</param>
        /// <param name="input">Dados do Palestrante</param>
        /// <returns>Nada</returns>
        /// <response code="200">Success</response>
        /// <response code="204">No Content</response>
        [HttpPost("{id}/speakers")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public IActionResult PostSpeaker(Guid id, DevEventSpeakerInputModel input)
        {
            var devEvent = _context.DevEvents.Any(d => d.Id == id);

            if (!devEvent)
            {
                return NoContent();
            }

            var speaker = _mapper.Map<DevEventSpeaker>(input);
            speaker.DevEventId = id;

            _context.DevEventSpeakers.Add(speaker);
            _context.SaveChanges();

            return Ok();
        }
    }
}
