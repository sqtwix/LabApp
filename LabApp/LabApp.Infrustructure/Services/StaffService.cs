using LabApp.Application.Interfaces;
using LabApp.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using LabApp.Domain.Entities;

namespace LabApp.Infrustructure.Services;

public class StaffService : IStaffService
{
    private readonly LabContext _context;

    public StaffService(LabContext context)
    {
        _context = context;
    }

    public async Task<Staff> GetStaffByIdAsync(int id)
    {
        return await _context.Staffs
            .Include(s => s.Position)
            .Include(s => s.City)
            .FirstOrDefaultAsync(s => s.StaffId == id);
    }

    public async Task<IEnumerable<Staff>> GetAllStaffAsync()
    {
        return await _context.Staffs
            .Include(s => s.Position)
            .Include(s => s.City)
            .ToListAsync();
    }

    public async Task<Staff> CreateStaffAsync(Staff staff)
    {
        _context.Staffs.Add(staff);
        await _context.SaveChangesAsync();
        return staff;
    }

    public async Task<Staff> UpdateStaffAsync(Staff staff)
    {
        _context.Entry(staff).State = EntityState.Modified;
        await _context.SaveChangesAsync();
        return staff;
    }

    public async Task<bool> DeleteStaffAsync(int id)
    {
        var staff = await _context.Staffs.FindAsync(id);
        if (staff == null) return false;
        _context.Staffs.Remove(staff);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<IEnumerable<Position>> GetAllPositionsAsync()
    {
        return await _context.Positions.ToListAsync();
    }

    public async Task<IEnumerable<Department>> GetAllDepartmentsAsync()
    {
        return await _context.Departments.ToListAsync();
    }

    public async Task<Department> GetDepartmentByIdAsync(int id)
    {
        return await _context.Departments.FindAsync(id);
    }

    public async Task AddStaffToDepartmentAsync(int staffId, int departmentId)
    {
        var exists = await _context.StaffDepartments
            .AnyAsync(sd => sd.StaffId == staffId && sd.DepartmentId == departmentId);
        if (!exists)
        {
            _context.StaffDepartments.Add(new StaffDepartment
            {
                StaffId = staffId,
                DepartmentId = departmentId
            });
            await _context.SaveChangesAsync();
        }
    }

    public async Task RemoveStaffFromDepartmentAsync(int staffId, int departmentId)
    {
        var link = await _context.StaffDepartments
            .FirstOrDefaultAsync(sd => sd.StaffId == staffId && sd.DepartmentId == departmentId);
        if (link != null)
        {
            _context.StaffDepartments.Remove(link);
            await _context.SaveChangesAsync();
        }
    }

    public async Task<IEnumerable<Specialization>> GetAllSpecializationsAsync()
    {
        return await _context.Specializations.ToListAsync();
    }

    public async Task AddSpecializationToStaffAsync(int staffId, int specializationId)
    {
        var exists = await _context.StaffSpecializations
            .AnyAsync(ss => ss.StaffId == staffId && ss.SpecializationId == specializationId);
        if (!exists)
        {
            _context.StaffSpecializations.Add(new StaffSpecialization
            {
                StaffId = staffId,
                SpecializationId = specializationId
            });
            await _context.SaveChangesAsync();
        }
    }

    public async Task RemoveSpecializationFromStaffAsync(int staffId, int specializationId)
    {
        var link = await _context.StaffSpecializations
            .FirstOrDefaultAsync(ss => ss.StaffId == staffId && ss.SpecializationId == specializationId);
        if (link != null)
        {
            _context.StaffSpecializations.Remove(link);
            await _context.SaveChangesAsync();
        }
    }

    public async Task<IEnumerable<StaffSchedule>> GetScheduleForStaffAsync(int staffId)
    {
        return await _context.StaffSchedules
            .Where(ss => ss.StaffId == staffId)
            .Include(ss => ss.Room)
            .ToListAsync();
    }

    public async Task<StaffSchedule> AddScheduleAsync(StaffSchedule schedule)
    {
        _context.StaffSchedules.Add(schedule);
        await _context.SaveChangesAsync();
        return schedule;
    }

    public async Task<StaffSchedule> UpdateScheduleAsync(StaffSchedule schedule)
    {
        _context.Entry(schedule).State = EntityState.Modified;
        await _context.SaveChangesAsync();
        return schedule;
    }

    public async Task<bool> DeleteScheduleAsync(int scheduleId)
    {
        var schedule = await _context.StaffSchedules.FindAsync(scheduleId);
        if (schedule == null) return false;
        _context.StaffSchedules.Remove(schedule);
        await _context.SaveChangesAsync();
        return true;
    }
}