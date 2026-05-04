using System;

namespace api.Interfaces;

public interface IBancoRepository
{
    public Task<Bancos> CreateAsync(BancosDTO)
    public Task<List<Bancos>> GetAllAsync()
    public Task<List<Bancos?> GetByAsync(int id);
    public Task<List<Bancos>> PostAsync(Bancos ban);
    public Task<Bancos?> UpdateAsync(int id, UpdateBancoRequestDTO updateClassroomRequest);
    public Task<Bancos?> DeleteAsync(int id);
   




}
