using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace dotnet_RPG.Services.Fight
{
    public class FightService : IFightService
    {
        private readonly DataContext _context;
        public FightService(DataContext context)
        {
            _context = context;
        }

        public async Task<ServiceResponse<FightResultDto>> Fight(FightRequestDto request)
        {
            var response = new ServiceResponse<FightResultDto> {
                Data = new FightResultDto()
            };
            try
            {
                var characters = await _context.Characters
                    .Include(c => c.Weapon)
                    .Include(c => c.Skills)
                    .Where(c => request.CharacterIds.Contains(c.Id))
                    .ToListAsync();

                bool defeated = false;

                while(!defeated) {
                    foreach(var attacker in characters) {

                        var opponents = characters.Where(c => c.Id != attacker.Id).ToList();
                        var opponent = opponents[new Random().Next(opponents.Count)];

                        int damage = 0;
                        string weaponUsed = string.Empty;
                        bool useWeapon = new Random().Next(2) == 0;
                        string attackUsed = string.Empty;

                        if(useWeapon && attacker.Weapon is not null) {

                            damage = attacker.Weapon.Damage + new Random().Next(attacker.strength);
                            damage -= new Random().Next(attacker.defense);
                            attackUsed = attacker.Weapon.Name;
                            
                            if(damage > 0) {
                                opponent.hitPoints -= damage;
                            }
                        }
                        else if (!useWeapon && attacker.Skills is not null) {
                            var skill = attacker.Skills[new Random().Next(attacker.Skills.Count)];
                            damage = skill.Damage + new Random().Next(attacker.intelligence);
                            damage -= new Random().Next(opponent.defense);
                            attackUsed = skill.Name;
                            if(damage > 0) {
                                opponent.hitPoints -= damage;
                            }
                        }
                        else {
                            response.Data.Log.Add($"{attacker.Name} was not able to attack");
                            continue;
                        }

                        response.Data.Log.Add($"{attacker.Name} attacks {opponent.Name} using {attackUsed} with {(damage >= 0 ? damage : 0)} damage");

                        if(opponent.hitPoints < 0) {
                            defeated = true;
                            attacker.Victories++;
                            opponent.Defeats++;
                            response.Data.Log.Add($"{opponent.Name} has been defeated.");
                            response.Data.Log.Add($"{attacker.Name} wins by {attacker.hitPoints} HP.");
                        }

                    }
                }
                
                characters.ForEach(c => {
                    c.hitPoints = 100;
                    c.Fights++;
                });

                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = ex.Message;
            }
            return response;
        }

        public async Task<ServiceResponse<AttackResultDto>> SkillAttack(SkillAttackDto request)
        {
            var response = new ServiceResponse<AttackResultDto>();

            try
            {
                var attacker = await _context.Characters
                    .Include(c => c.Skills)
                    .FirstOrDefaultAsync(c => c.Id == request.AttackerId);
                
                var opponent = await _context.Characters
                    .FirstOrDefaultAsync(c => c.Id == request.OpponentId);

                if (attacker is null || opponent is null || attacker.Skills is null) {
                    throw new Exception("Something fishy going on over here....");
                }

                var skill = attacker.Skills.FirstOrDefault(s => s.Id == request.SkillId);
                
                if (skill is null) 
                    throw new Exception($"{opponent.Name} does not know the skill!!!");

                int damage = skill.Damage + new Random().Next(attacker.intelligence);
                damage -= new Random().Next(opponent.defense);

                if(damage > 0) {
                    opponent.hitPoints -= damage;
                }

                if (opponent.hitPoints <= 0) {
                    response.Message = $"{opponent.Name} has been defeated.";
                }

                await _context.SaveChangesAsync();
                response.Data = new AttackResultDto {
                    Attacker = attacker.Name,
                    Opponent = opponent.Name,
                    AttackerHP = attacker.hitPoints,
                    OpponentHP = opponent.hitPoints,
                    Damage = damage
                };
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = ex.Message;
            }
            return response;
        }

        public async Task<ServiceResponse<AttackResultDto>> WeaponAttack(WeaponAttackDto request)
        {
            var response = new ServiceResponse<AttackResultDto>();

            try
            {
                var attacker = await _context.Characters
                    .Include(c => c.Weapon)
                    .FirstOrDefaultAsync(c => c.Id == request.AttackerId);
                
                var opponent = await _context.Characters
                    .FirstOrDefaultAsync(c => c.Id == request.OpponentId);

                if (attacker is null || opponent is null || attacker.Weapon is null) {
                    throw new Exception("Something fishy going on over here....");
                }

                int damage = attacker.Weapon.Damage + new Random().Next(attacker.strength);
                damage -= new Random().Next(opponent.defense);

                if (damage > 0) {
                    opponent.hitPoints -= damage;
                }

                if (opponent.hitPoints <= 0) {
                    response.Message = $"{opponent.Name} has been defeated.";
                }

                await _context.SaveChangesAsync();
                response.Data = new AttackResultDto {
                    Attacker = attacker.Name,
                    Opponent = opponent.Name,
                    AttackerHP = attacker.hitPoints,
                    OpponentHP = opponent.hitPoints,
                    Damage = damage
                };
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = ex.Message;
            }
            return response;
        }
    }
}