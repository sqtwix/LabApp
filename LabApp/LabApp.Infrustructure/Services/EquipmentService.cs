using LabApp.Application.Interfaces;
using LabApp.Domain.Entities;
using LabApp.Infrustructure;
using Microsoft.EntityFrameworkCore;

namespace LabApp.Infrastructure.Services;

public class EquipmentService : IEquipmentService
{
    private readonly LabContext _context;

    public EquipmentService(LabContext context)
    {
        _context = context;
    }

    public async Task<Equipment> GetEquipmentByIdAsync(int id)
    {
        return await _context.Equipment
            .Include(e => e.Room)
            .FirstOrDefaultAsync(e => e.EquipmentId == id);
    }

    public async Task<IEnumerable<Equipment>> GetAllEquipmentAsync()
    {
        return await _context.Equipment
            .Include(e => e.Room)
            .ToListAsync();
    }

    public async Task<Equipment> CreateEquipmentAsync(Equipment equipment)
    {
        _context.Equipment.Add(equipment);
        await _context.SaveChangesAsync();
        return equipment;
    }

    public async Task<Equipment> UpdateEquipmentAsync(Equipment equipment)
    {
        _context.Entry(equipment).State = EntityState.Modified;
        await _context.SaveChangesAsync();
        return equipment;
    }

    public async Task<bool> DeleteEquipmentAsync(int id)
    {
        var equipment = await _context.Equipment.FindAsync(id);
        if (equipment == null) return false;
        _context.Equipment.Remove(equipment);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<Reagent> GetReagentByIdAsync(int id)
    {
        return await _context.Reagents.FindAsync(id);
    }

    public async Task<IEnumerable<Reagent>> GetAllReagentsAsync()
    {
        return await _context.Reagents.ToListAsync();
    }

    public async Task<Reagent> CreateReagentAsync(Reagent reagent)
    {
        _context.Reagents.Add(reagent);
        await _context.SaveChangesAsync();
        return reagent;
    }

    public async Task<bool> DeleteReagentAsync(int id)
    {
        var reagent = await _context.Reagents.FindAsync(id);
        if (reagent == null) return false;
        _context.Reagents.Remove(reagent);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<IEnumerable<Equipment>> GetEquipmentForResearchAsync(int researchId)
    {
        var research = await _context.Researches
            .Include(r => r.Equipment)
            .FirstOrDefaultAsync(r => r.ResearchId == researchId);

        return research?.Equipment ?? new List<Equipment>();
    }

    public async Task AssignEquipmentToResearchAsync(int researchId, int equipmentId)
    {
        var research = await _context.Researches
            .Include(r => r.Equipment)
            .FirstOrDefaultAsync(r => r.ResearchId == researchId);
        var equipment = await _context.Equipment.FindAsync(equipmentId);

        if (research == null || equipment == null)
            return;

        if (!research.Equipment.Any(e => e.EquipmentId == equipmentId))
        {
            research.Equipment.Add(equipment);
            await _context.SaveChangesAsync();
        }
    }

    public async Task UnassignEquipmentFromResearchAsync(int researchId, int equipmentId)
    {
        var research = await _context.Researches
            .Include(r => r.Equipment)
            .FirstOrDefaultAsync(r => r.ResearchId == researchId);

        if (research == null)
            return;

        var equipment = research.Equipment.FirstOrDefault(e => e.EquipmentId == equipmentId);
        if (equipment != null)
        {
            research.Equipment.Remove(equipment);
            await _context.SaveChangesAsync();
        }
    }
}