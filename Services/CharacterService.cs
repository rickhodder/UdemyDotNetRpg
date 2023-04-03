global using AutoMapper;

namespace Services;
public class CharacterService : ICharacterService
{
    private static List<Character> characters=new List<Character>
    {
        new Character(),
        new Character{Id=1,Name="Sam"}
    };
    private readonly IMapper _mapper;

    public CharacterService(IMapper mapper)
    {
        _mapper = mapper;
    }

    public  async Task<ServiceResponse<List<GetCharacterDto>>> AddCharacter(AddCharacterDto newCharacter)
    {
        var character = _mapper.Map<Character>(newCharacter);
        character.Id=characters.Max(c=>c.Id)+1;
        characters.Add(_mapper.Map<Character>(character));
        return new ServiceResponse<List<GetCharacterDto>>{Data=characters.Select(c=>_mapper.Map<GetCharacterDto>(c)).ToList()};
    }

    public async Task<ServiceResponse<List<GetCharacterDto>>> DeleteCharacter(int id)
    {
        var characterResponse  = new ServiceResponse<List<GetCharacterDto>>();
        try
        {
            var character = characters.FirstOrDefault(c=>c.Id==id);
            if(character==null)
                throw new Exception($"Character with id {id} not found");

            characters.Remove(character);           
            characterResponse.Data=characters.Select(c=>_mapper.Map<GetCharacterDto>(c)).ToList();
        }
        catch (System.Exception ex)
        {
            characterResponse.Success=false;
            characterResponse.Message=ex.Message;
            
        }
                
        return characterResponse;
    }

    public  async Task<ServiceResponse<List<GetCharacterDto>>> GetAllCharacters()
    {
        var data = characters.Select(c=>_mapper.Map<GetCharacterDto>(c)).ToList();
        return new ServiceResponse<List<GetCharacterDto>>{Data= data};
    }

    public async Task<ServiceResponse<GetCharacterDto>> GetCharacterById(int id)
    {
        var character = characters.FirstOrDefault(c => c.Id == id);
        if(character==null)
            throw new Exception("Character not found");

        return new ServiceResponse<GetCharacterDto>{Data=_mapper.Map<GetCharacterDto>(character)};
    }

    public async Task<ServiceResponse<GetCharacterDto>> UpdateCharacter(UpdateCharacterDto updatedCharacter)
    {
        var characterResponse  = new ServiceResponse<GetCharacterDto>();
        try
        {
            var character = characters.FirstOrDefault(c=>c.Id==updatedCharacter.Id);
            if(character==null)
                throw new Exception($"Character with id {updatedCharacter.Id} not found");

            character.Name=updatedCharacter.Name;
            character.Class=updatedCharacter.Class;
            character.Defense=updatedCharacter.Defense;
            character.HitPoints=updatedCharacter.HitPoints;
            character.Intelligence=updatedCharacter.Intelligence;
            character.Strength=updatedCharacter.Strength;
            
        }
        catch (System.Exception ex)
        {
            characterResponse.Success=false;
            characterResponse.Message=ex.Message;
            
        }
                
        return characterResponse;
    }
}