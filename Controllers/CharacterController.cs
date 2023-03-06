using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace dotnet_RPG.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class CharacterController : ControllerBase
    {
        private readonly ICharacterService _characterService;

        public CharacterController(ICharacterService characterService) 
        {
           _characterService = characterService;
        }

        [HttpGet("GetAll")]
        public async Task<ActionResult<ServiceResponse<List<GetCharacterDto>>>> Get() {
            return Ok(await _characterService.GetAllCharacters());
        }

        [HttpDelete("Delete{id}")]
        public async Task<ActionResult<ServiceResponse<List<GetCharacterDto>>>> Delete(int id) {
            var response = await _characterService.DeleteCharacter(id);
            
            if(response.Data is null) {
                return NotFound(response);
            }
            return Ok(response);
        }

        [HttpGet("GetSingle{id}")]
        public async Task<ActionResult<ServiceResponse<GetCharacterDto>>> GetSingle(int id) {
            return Ok(await _characterService.GetCharacterById(id));
        }

        [HttpPost("Add")]
        public async Task<ActionResult<ServiceResponse<List<GetCharacterDto>>>> Add(AddCharacterDto newCharacter) {
            return Ok(await _characterService.AddCharacter(newCharacter));
        }

        [HttpPut("Update")]
        public async Task<ActionResult<ServiceResponse<GetCharacterDto>>> Update(UpdateCharacterDto updateCharacter) {
            
            var response = await _characterService.UpdateCharacter(updateCharacter);
            
            if(response.Data is null) {
                return NotFound(response);
            }
            return Ok(response);
        }

        [HttpPost("Skill")]
        public async Task<ActionResult<ServiceResponse<GetCharacterDto>>> AddCharacterSkill(AddCharacterSkillDto newCharacterSkill) {
            return Ok(await _characterService.AddCharacterSkill(newCharacterSkill));
        }
    }
}