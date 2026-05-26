using LabApp.Domain.Entities;


namespace LabApp.Application.Interfaces;

public interface IEquipmentService
{
    // Оборудование
    Task<Equipment> GetEquipmentByIdAsync(int id);
    Task<IEnumerable<Equipment>> GetAllEquipmentAsync();
    Task<Equipment> CreateEquipmentAsync(Equipment equipment);
    Task<Equipment> UpdateEquipmentAsync(Equipment equipment);
    Task<bool> DeleteEquipmentAsync(int id);

    // Реагенты
    Task<Reagent> GetReagentByIdAsync(int id);
    Task<IEnumerable<Reagent>> GetAllReagentsAsync();
    Task<Reagent> CreateReagentAsync(Reagent reagent);
    Task<bool> DeleteReagentAsync(int id);

    // Связь исследования – оборудование
    Task<IEnumerable<Equipment>> GetEquipmentForResearchAsync(int researchId);
    Task AssignEquipmentToResearchAsync(int researchId, int equipmentId);
    Task UnassignEquipmentFromResearchAsync(int researchId, int equipmentId);
    Task<IEnumerable<Room>> GetAllRoomsAsync();
}

