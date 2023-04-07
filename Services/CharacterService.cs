global using AutoMapper;
using System.Security.Claims;
using Microsoft.EntityFrameworkCore;

namespace Services;
public class CharacterService : ICharacterService
{

    private readonly IMapper _mapper;
    private readonly DataContext _context;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public CharacterService(IMapper mapper, DataContext context, IHttpContextAccessor httpContextAccessor)
    {
        _mapper = mapper;
        _context = context;
        _httpContextAccessor = httpContextAccessor;
    }

    private int GetUserId() 
    {
        return int.Parse(_httpContextAccessor.HttpContext!.User
        .FindFirstValue(ClaimTypes.NameIdentifier)!);
    }

    public async Task<ServiceResponse<List<GetCharacterDto>>> AddCharacter(AddCharacterDto newCharacter)
    {
        var character = _mapper.Map<Character>(newCharacter);
        character.User = await _context.Users.FirstOrDefaultAsync(u=>u.Id==GetUserId());

        _context.Characters.Add(character);
        await _context.SaveChangesAsync();
        var response = new ServiceResponse<List<GetCharacterDto>>();
        response.Data = await _context.Characters
            .Where(c => c.User!.Id == GetUserId())
            .Select(c => _mapper.Map<GetCharacterDto>(c))
            .ToListAsync();

        return response;
    }

    public async Task<ServiceResponse<List<GetCharacterDto>>> DeleteCharacter(int id)
    {
        var characterResponse = new ServiceResponse<List<GetCharacterDto>>();
        try
        {
            var character = await _context.Characters
                .FirstOrDefaultAsync(c => c.Id == id && c.User!.Id==GetUserId());
            if (character == null)
                throw new Exception($"Character with id {id} not found");

            _context.Characters.Remove(character);
            await _context.SaveChangesAsync();

            characterResponse.Data = await _context.Characters
                .Where(c => c.User!.Id == GetUserId())
                .Select(c => _mapper.Map<GetCharacterDto>(c)).ToListAsync();
        }
        catch (System.Exception ex)
        {
            characterResponse.Success = false;
            characterResponse.Message = ex.Message;

        }

        return characterResponse;
    }

    public async Task<ServiceResponse<List<GetCharacterDto>>> GetAllCharacters()
    {
        
        var dbCharacters = await _context.Characters.Where(c => c.User!.Id == GetUserId()).ToListAsync();
        var data = dbCharacters.Select(c => _mapper.Map<GetCharacterDto>(c)).ToList();
        return new ServiceResponse<List<GetCharacterDto>> { Data = data };
    }

    public async Task<ServiceResponse<GetCharacterDto>> GetCharacterById(int id)
    {
        var dbCharacter = await _context.Characters            
            .FirstOrDefaultAsync(c => c.Id == id && c.User!.Id == GetUserId());
        if (dbCharacter == null)
            throw new Exception("Character not found");

        return new ServiceResponse<GetCharacterDto> { Data = _mapper.Map<GetCharacterDto>(dbCharacter) };
    }

    public async Task<ServiceResponse<GetCharacterDto>> UpdateCharacter(UpdateCharacterDto updatedCharacter)
    {
        var characterResponse = new ServiceResponse<GetCharacterDto>();
        try
        {
            var character =
                await _context.Characters
                    .Include(c => c.User)
                    .FirstOrDefaultAsync(c => c.Id == updatedCharacter.Id);
            if (character == null || character.User!.Id!= GetUserId())
                throw new Exception($"Character with id {updatedCharacter.Id} not found");

            character.Name = updatedCharacter.Name;
            character.Class = updatedCharacter.Class;
            character.Defense = updatedCharacter.Defense;
            character.HitPoints = updatedCharacter.HitPoints;
            character.Intelligence = updatedCharacter.Intelligence;
            character.Strength = updatedCharacter.Strength;
            await _context.SaveChangesAsync();
        }
        catch (System.Exception ex)
        {
            characterResponse.Success = false;
            characterResponse.Message = ex.Message;

        }

        return characterResponse;
    }
}