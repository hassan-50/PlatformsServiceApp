using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using PlatformService.AsyncDataServices;
using PlatformService.Data;
using PlatformService.Dtos;
using PlatformService.Models;
using PlatformService.SyncDataServices.Http;

namespace PlatformService.Controllers;
[Route("api/[controller]")]
[ApiController]
public class PlatformsController : ControllerBase{
    private readonly ICommandDataClient _commandDataClient;
    private readonly IPlatformRepo _platformRepo;
    private readonly IMapper _mapper;
    private readonly IMessageBusClient _messageBusClient;

    public PlatformsController(IPlatformRepo platformRepo , IMapper mapper, ICommandDataClient commandDataClient
    ,IMessageBusClient messageBusClient)
    {
    _commandDataClient = commandDataClient;
    _platformRepo = platformRepo;
    _mapper = mapper;
    _messageBusClient = messageBusClient; 
    }

    [HttpGet]
    public ActionResult<IEnumerable<PlatformReadDto>> GetPlatforms()
    {
        var platForms = _platformRepo.GetAllPlatforms();
        return Ok(_mapper.Map<IEnumerable<PlatformReadDto>>(platForms));
    }

    [HttpGet("{id}",Name="GetPlatformById")]
    public ActionResult<PlatformReadDto> GetPlatformById(int id)
    {
        var platForm = _platformRepo.GetPlatformById(id);
        if(platForm != null){
        return Ok(_mapper.Map<PlatformReadDto>(platForm));
        }
        return NotFound();
    }

    [HttpPost]
    public async Task<ActionResult<PlatformReadDto>> CreatePlatform(PlatformCreateDto platformCreateDto)
    {
        var platformModel = _mapper.Map<Platform>(platformCreateDto);
        _platformRepo.CreatePlatform(platformModel);
        _platformRepo.SaveChanges();

        var platformReadDto = _mapper.Map<PlatformReadDto>(platformModel);
        // Send Sync Message 
        try
        {
        await _commandDataClient.SendPlatformsToCommand(platformReadDto);
        }
        catch (Exception e)
        {               
        Console.WriteLine($"--> Could not send Synchronously: {e.Message}");          
        }

        // Send ASync Message 
        try
        {
            var platformPublishedDto = _mapper.Map<PlatformPublishedDto>(platformReadDto);
            platformPublishedDto.Event = "Platform_Published";
            _messageBusClient.PublishNewPlatform(platformPublishedDto);
        }
        catch (Exception e)
        {               
        Console.WriteLine($"--> Could not send Synchronously: {e.Message}");          
        }

        return CreatedAtRoute(nameof(GetPlatformById),new { Id = platformReadDto.Id },platformReadDto);
    }

}