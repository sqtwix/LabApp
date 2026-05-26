using LabApp.Application.Interfaces;
using LabApp.Domain.Entities;
using LabApp.Infrustructure;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

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

    // НОВЫЙ МЕТОД ДЛЯ COMBOBOX С КАБИНЕТАМИ
    public async Task<IEnumerable<Room>> GetAllRoomsAsync()
    {
        return await _context.Rooms.ToListAsync();
    }

    public async Task<Equipment> CreateEquipmentAsync(Equipment equipment)
    {
        // Ручная генерация ID, так как в БД нет SERIAL
        var maxId = await _context.Equipment.MaxAsync(e => (int?)e.EquipmentId) ?? 0;
        equipment.EquipmentId = maxId + 1;

        _context.Equipment.Add(equipment);
        await _context.SaveChangesAsync();

        // Перезапрашиваем, чтобы подтянуть объект Room для UI
        return await GetEquipmentByIdAsync(equipment.EquipmentId);
    }

    public async Task<Equipment> UpdateEquipmentAsync(Equipment equipment)
    {
        var existing = await _context.Equipment.FindAsync(equipment.EquipmentId);
        if (existing == null) return null;

        existing.Name = equipment.Name;
        existing.RoomId = equipment.RoomId;
        existing.ExplotationDate = equipment.ExplotationDate;
        existing.ServiceLife = equipment.ServiceLife;

        await _context.SaveChangesAsync();
        return existing;
    }

    public async Task<bool> DeleteEquipmentAsync(int id)
    {
        var equipment = await _context.Equipment.FindAsync(id);
        if (equipment == null) return false;
        _context.Equipment.Remove(equipment);
        await _context.SaveChangesAsync();
        return true;
    }

    // --- ОСТАЛЬНЫЕ МЕТОДЫ (Без изменений) ---
    public async Task<Reagent> GetReagentByIdAsync(int id) => await _context.Reagents.FindAsync(id);
    public async Task<IEnumerable<Reagent>> GetAllReagentsAsync() => await _context.Reagents.ToListAsync();
    public async Task<Reagent> CreateReagentAsync(Reagent reagent) { _context.Reagents.Add(reagent); await _context.SaveChangesAsync(); return reagent; }
    public async Task<bool> DeleteReagentAsync(int id) { var r = await _context.Reagents.FindAsync(id); if (r == null) return false; _context.Reagents.Remove(r); await _context.SaveChangesAsync(); return true; }

    public async Task<IEnumerable<Equipment>> GetEquipmentForResearchAsync(int researchId)
    {
        var research = await _context.Researches.Include(r => r.Equipment).FirstOrDefaultAsync(r => r.ResearchId == researchId);
        return research?.Equipment ?? new List<Equipment>();
    }

    public async Task AssignEquipmentToResearchAsync(int researchId, int equipmentId)
    {
        var research = await _context.Researches.Include(r => r.Equipment).FirstOrDefaultAsync(r => r.ResearchId == researchId);
        var equipment = await _context.Equipment.FindAsync(equipmentId);
        if (research != null && equipment != null && !research.Equipment.Any(e => e.EquipmentId == equipmentId))
        {
            research.Equipment.Add(equipment);
            await _context.SaveChangesAsync();
        }
    }

    public async Task UnassignEquipmentFromResearchAsync(int researchId, int equipmentId)
    {
        var research = await _context.Researches.Include(r => r.Equipment).FirstOrDefaultAsync(r => r.ResearchId == researchId);
        var equipment = research?.Equipment.FirstOrDefault(e => e.EquipmentId == equipmentId);
        if (equipment != null)
        {
            research.Equipment.Remove(equipment);
            await _context.SaveChangesAsync();
        }
    }
}