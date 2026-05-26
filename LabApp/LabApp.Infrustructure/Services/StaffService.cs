using LabApp.Application.Interfaces;
using LabApp.Domain.Entities;
using Npgsql;
using LabApp.Infrastructure.Helpers;
using Microsoft.EntityFrameworkCore;

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
        var staff = await _context.Staffs
            .Include(s => s.Position)
            .Include(s => s.City)
            .FirstOrDefaultAsync(s => s.StaffId == id);
        if (staff != null)
        {
            staff.Address = EncryptionHelper.Decrypt(staff.Address);
            staff.Passport = EncryptionHelper.Decrypt(staff.Passport);
        }
        return staff;
    }

    public async Task<IEnumerable<Staff>> GetAllStaffAsync()
    {
        var staffList = await _context.Staffs
            .Include(s => s.Position)
            .Include(s => s.City)
            .ToListAsync();
        foreach (var s in staffList)
        {
            s.Address = EncryptionHelper.Decrypt(s.Address);
            s.Passport = EncryptionHelper.Decrypt(s.Passport);
        }
        return staffList;
    }

    public async Task<Staff> CreateStaffAsync(Staff staff)
    {
        // Шифруем перед передачей в функцию
        var encryptedPassport = EncryptionHelper.Encrypt(staff.Passport ?? "");
        var encryptedAddress = EncryptionHelper.Encrypt(staff.Address ?? "");
        var tempPassword = "temp123";
        var birthDateStr = staff.BirthDate?.ToString("yyyy-MM-dd");

        var sql = @"
        SELECT add_staff(
            @p0, @p1, @p2, @p3, @p4, @p5, @p6,
            @p7::date, @p8, @p9, @p10, @p11, @p12, @p13
        )";

        using var command = _context.Database.GetDbConnection().CreateCommand();
        command.CommandText = sql;
        command.Parameters.Add(new NpgsqlParameter("@p0", staff.LastName ?? (object)DBNull.Value));
        command.Parameters.Add(new NpgsqlParameter("@p1", staff.FirstName ?? (object)DBNull.Value));
        command.Parameters.Add(new NpgsqlParameter("@p2", staff.MiddleName ?? (object)DBNull.Value));
        command.Parameters.Add(new NpgsqlParameter("@p3", staff.Gender ?? (object)DBNull.Value));
        command.Parameters.Add(new NpgsqlParameter("@p4", staff.Phone ?? (object)DBNull.Value));
        command.Parameters.Add(new NpgsqlParameter("@p5", encryptedPassport ?? (object)DBNull.Value));
        command.Parameters.Add(new NpgsqlParameter("@p6", encryptedAddress ?? (object)DBNull.Value));
        command.Parameters.Add(new NpgsqlParameter("@p7", birthDateStr ?? (object)DBNull.Value));
        command.Parameters.Add(new NpgsqlParameter("@p8", staff.PositionId ?? (object)DBNull.Value));
        command.Parameters.Add(new NpgsqlParameter("@p9", staff.CityId ?? (object)DBNull.Value));
        command.Parameters.Add(new NpgsqlParameter("@p10", staff.Education ?? (object)DBNull.Value));
        command.Parameters.Add(new NpgsqlParameter("@p11", staff.Login ?? (object)DBNull.Value));
        command.Parameters.Add(new NpgsqlParameter("@p12", tempPassword));
        command.Parameters.Add(new NpgsqlParameter("@p13", staff.RoleName ?? (object)DBNull.Value));

        await _context.Database.OpenConnectionAsync();
        var staffId = Convert.ToInt32(await command.ExecuteScalarAsync());
        await _context.Database.CloseConnectionAsync();

        staff.StaffId = staffId;
        staff.PasswordHash = tempPassword;

        // Расшифровываем для отображения
        staff.Passport = EncryptionHelper.Decrypt(encryptedPassport);
        staff.Address = EncryptionHelper.Decrypt(encryptedAddress);

        return staff;
    }

    public async Task<Staff> UpdateStaffAsync(Staff staff)
    {
        // Сохраняем незашифрованные данные
        var plainAddress = staff.Address;
        var plainPassport = staff.Passport;

        try
        {
            staff.Address = EncryptionHelper.Encrypt(plainAddress ?? "");
            staff.Passport = EncryptionHelper.Encrypt(plainPassport ?? "");

            _context.Staffs.Update(staff);
            await _context.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            throw new Exception("Ошибка при обновлении сотрудника в БД: " + ex.Message);
        }
        finally
        {
            // Независимо от того, успешно прошло сохранение или была ошибка,
            // возвращаем в UI нормальные, расшифрованные данные
            staff.Address = plainAddress;
            staff.Passport = plainPassport;
        }

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

    // ... остальные методы (GetAllPositionsAsync, GetAllCitiesAsync и т.д.) без изменений

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

    public async Task<IEnumerable<City>> GetAllCitiesAsync()
    {
        return await _context.Cities.ToListAsync();
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