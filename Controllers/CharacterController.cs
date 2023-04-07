using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

[Authorize]
[ApiController]
[Route("api/[controller]")] // api/Character
public class CharacterController:ControllerBase // since no ui
{
    private ICharacterService _characterService;

    public CharacterController(ICharacterService characterService)
    {
        _characterService=characterService;
    }

    [HttpGet("GetAll")]
    public async Task<ActionResult<ServiceResponse<List<GetCharacterDto>>>> Get()
    {
        int userid = int.Parse(User.Claims.FirstOrDefault(c=>c.Type==ClaimTypes.NameIdentifier)!.Value);
        return Ok(await _characterService.GetAllCharacters());
    }

    [HttpGet("{id}")]
     public  async Task<ActionResult<ServiceResponse<GetCharacterDto>>> GetSingle(int id)
    {
        return Ok(await _characterService.GetCharacterById(id));
    }

    [HttpPost]
    public  async Task<ActionResult<ServiceResponse<List<GetCharacterDto>>>> AddCharacter(AddCharacterDto newCharacter)
    {
        return Ok(await _characterService.AddCharacter(newCharacter));
    }

    [HttpPut]
    public async Task<ActionResult<ServiceResponse<GetCharacterDto>>> UpdateCharacter(UpdateCharacterDto updatedCharacter)
    {
        var response=await _characterService.UpdateCharacter(updatedCharacter);
        
        if(!response.Success)
            return NotFound(response);
        return Ok(response);
    }


    [HttpDelete("{id}")]
    public async Task<ActionResult<ServiceResponse<List<GetCharacterDto>>>> DeleteCharacter(int id)
    {
        var response=await _characterService.DeleteCharacter(id);
        
        if(!response.Success)
            return NotFound(response);
        return Ok(response);
    }

    [HttpPost("Skill")]
    public async Task<ActionResult<ServiceResponse<GetCharacterDto>>> AddCharacterSkill(AddCharacterSkillDto newCharacterSkill)
    {
        return Ok( await _characterService.AddCharacterSkill(newCharacterSkill));
    }
}