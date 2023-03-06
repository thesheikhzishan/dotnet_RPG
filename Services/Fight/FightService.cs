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