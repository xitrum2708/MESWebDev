using AutoMapper;
using MESWebDev.Data;
using MESWebDev.Models.Master;
using MESWebDev.Models.Master.DTO;
using MESWebDev.Services.IService;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace MESWebDev.Services
{
    public class AuthService : IAuthService
    {
        private readonly IMapper _map;
        private readonly AppDbContext _context;
        private readonly IHttpContextAccessor _hca;

        public AuthService(IMapper map, AppDbContext context, IHttpContextAccessor hca)
        {
            _map = map;
            _context = context;
            _hca = hca;
        }

        #region ---------------------- USERS ----------------------

        public async Task<List<UserDTO>> GetAllUsersAsync(UserDTO? search = null)
        {
            List<UserDTO> result = new();
            var users =  _context.Auth_Master_User.AsQueryable();
            if (search != null) {
                if (!string.IsNullOrEmpty(search.Username))
                {
                    users = users.Where(u => u.Username.Contains(search.Username));
                }
                if (!string.IsNullOrEmpty(search.Email))
                {
                    users = users.Where(u => u.Email.Contains(search.Email));
                }
                if (!string.IsNullOrEmpty(search.Fullname))
                {
                    users = users.Where(u => u.Fullname != null && u.Fullname.Contains(search.Fullname));
                }
                if (search.RoleId != 0)
                {
                    users = users.Where(u => u.RoleId == search.RoleId);
                }
                if (search.IsActive)
                {
                    users = users.Where(u => u.IsActive == search.IsActive);
                }
            }
            if (users.Any())
            {
                var userDtos = _map.ProjectTo<UserDTO>(users);
                result = userDtos.ToList();
            }
            
            return result;
        }

        public async Task<string> CreateUserAsync(UserDTO userDto)
        {
            try
            {
                var user = _map.Map<UserModel>(userDto);
                user.CreatedBy = _hca.HttpContext.User.Identity.Name;
                user.CreatedDt = DateTime.Now;
                await _context.Auth_Master_User.AddAsync(user);
                await _context.SaveChangesAsync();

                return string.Empty;
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }
        public async Task<bool> DeleteUserAsync(string username)
        {
            var user = await _context.Auth_Master_User.FindAsync(username);
            if (user == null)
            {
                return false;
            }
            _context.Auth_Master_User.Remove(user);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<UserDTO?> GetUserByUsernameAsync(string username)
        {
            return _map.Map<UserDTO>(await _context.Auth_Master_User.FindAsync(username));
        }

        public async Task<string> UpdateUserAsync(UserDTO userDto)
        {
            try
            {
                var user = await _context.Auth_Master_User.FindAsync(userDto.Username);
                if (user == null)
                {
                    return "User not found.";
                }
                // Map updated fields from DTO to the existing entity
                _map.Map(userDto, user);
                user.UpdatedBy = _hca.HttpContext.User.Identity.Name;
                user.UpdatedDt = DateTime.Now;
                _context.Auth_Master_User.Update(user);
                await _context.SaveChangesAsync();
                return string.Empty;
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        #endregion ----------------------------------------


        #region ---------------------- Role ----------------------
        public async Task<List<RoleModel>> GetAllRolesAsync(RoleDTO? search = null)
        {
            var result = new List<RoleModel>();

            var roles = _context.Auth_Master_Role.AsQueryable();
            
            if (search != null)
            {
                if (search.RoleId  >0)
                {
                    roles = roles.Where(r => r.RoleId == search.RoleId);
                }
                if (!string.IsNullOrEmpty(search.RoleName))
                {
                    roles = roles.Where(r => r.RoleName == search.RoleName);
                }
            }
        
            if (roles.Any())
            {
                result = roles.ToList();
            }
            return result;
        }


        public async Task<string> CreateRoleAsync(RoleDTO roleDto)
        {
            var check = _context.Auth_Master_Role.FirstOrDefault(r => r.RoleName == roleDto.RoleName);
            if (check != null)
            {
                return "Role name already exists.";
            }
            var role = _map.Map<RoleModel>(roleDto);
            role.CreatedBy = _hca.HttpContext.User.Identity.Name;
            role.CreatedDt = DateTime.Now;
            await _context.Auth_Master_Role.AddAsync(role);
            await _context.SaveChangesAsync();
            return string.Empty;
        }
        public async Task<bool> DeleteRoleAsync(int roleId)
        {
            var role = await _context.Auth_Master_Role.FindAsync(roleId);
            if (role == null)
            {
                return false;
            }
            _context.Auth_Master_Role.Remove(role);
            await _context.SaveChangesAsync();
            return true;
        }
        public async Task<RoleDTO?> GetRoleByIdAsync(int roleId)
        {
            var role = await _context.Auth_Master_Role.FindAsync(roleId);
            return _map.Map<RoleDTO>(role);
        }
        public async Task<string> UpdateRoleAsync(RoleDTO roleDto)
        {
            var check = _context.Auth_Master_Role.FirstOrDefault(r => r.RoleName == roleDto.RoleName && r.RoleId != roleDto.RoleId);
            if (check != null)
            {
                return "Role name already exists.";
            }
            var role = await _context.Auth_Master_Role.FindAsync(roleDto.RoleId);
            if (role == null)
            {
                return "Role not found.";
            }
            _map.Map(roleDto, role);
            await _context.SaveChangesAsync();
            return string.Empty;
        }

        #endregion ----------------------------------------



        #region ---------------------- FUNCTIONS & ROLES & PERMISSIONS ----------------------

        public async Task<string> CreateRoleFuncPmsAsync(RoleFuncPmsDTO roleFuncPmsDto)
        {
            var check = _context.Auth_Mapping_Role_Func_Pms.FirstOrDefault(rfp =>
                rfp.RoleId == roleFuncPmsDto.RoleId &&
                rfp.FuncId == roleFuncPmsDto.FuncId &&
                rfp.PmsId == roleFuncPmsDto.PmsId);
            if (check != null)
            {
                return "Mapping already exists.";
            }
            var roleFuncPms = _map.Map<RoleFuncPmsModel>(roleFuncPmsDto);
            roleFuncPms.CreatedBy = _hca.HttpContext.User.Identity.Name;
            roleFuncPms.CreatedDt = DateTime.Now;
            await _context.Auth_Mapping_Role_Func_Pms.AddAsync(roleFuncPms);
            await _context.SaveChangesAsync();
            return string.Empty;           
        }

        public async Task<bool> DeleteRoleFuncPmsAsync(int id)
        {
            var roleFuncPms = await _context.Auth_Mapping_Role_Func_Pms.FindAsync(id);
            if (roleFuncPms == null)
            {
                return false;
            }
            _context.Auth_Mapping_Role_Func_Pms.Remove(roleFuncPms);
            await _context.SaveChangesAsync();
            return true;
        }
        public async Task<List<RoleFuncPmsDTO>> GetAllRoleFuncPmsAsync(RoleFuncPmsDTO search = null)
        {
            var result = new List<RoleFuncPmsDTO>();
            var roleFuncPms = _context.Auth_Mapping_Role_Func_Pms.AsQueryable();
            if (search != null)
            {
                if (search.RoleId != 0)
                {
                    roleFuncPms = roleFuncPms.Where(rfp => rfp.RoleId == search.RoleId);
                }
                if (search.FuncId != 0)
                {
                    roleFuncPms = roleFuncPms.Where(rfp => rfp.FuncId == search.FuncId);
                }
                if (search.PmsId != 0)
                {
                    roleFuncPms = roleFuncPms.Where(rfp => rfp.PmsId == search.PmsId);
                }
            }
            if (roleFuncPms.Any())
            {
                var roleFuncPmsDtos = _map.ProjectTo<RoleFuncPmsDTO>(roleFuncPms);
                result = roleFuncPmsDtos.OrderBy(i=>i.RoleName).ToList();
            }
            return result;
        }

        public async Task<string> UpdateRoleFuncPmsAsync(RoleFuncPmsDTO roleFuncPmsDto)
        {
            var check = _context.Auth_Mapping_Role_Func_Pms.FirstOrDefault(rfp =>
                rfp.RoleId == roleFuncPmsDto.RoleId &&
                rfp.FuncId == roleFuncPmsDto.FuncId &&
                rfp.PmsId == roleFuncPmsDto.PmsId &&
                rfp.Id != roleFuncPmsDto.Id);
            if (check != null)
            {
                return "Mapping already exists.";
            }
            var roleFuncPms = await _context.Auth_Mapping_Role_Func_Pms.FindAsync(roleFuncPmsDto.Id);
            if (roleFuncPms == null)
            {
                return "Mapping not found.";
            }
            _map.Map(roleFuncPmsDto, roleFuncPms);
            roleFuncPms.UpdatedBy = _hca.HttpContext.User.Identity.Name;
            roleFuncPms.UpdatedDt = DateTime.Now;
            await _context.SaveChangesAsync();
            return string.Empty;
        }

        #endregion ----------------------------------------


        #region ---------------------- Function ----------------------
        public Task<string> CreateFunctionAsync(FunctionDTO functionDto)
        {
            throw new NotImplementedException();
        }
        public Task<bool> DeleteFunctionAsync(int functionId)
        {
            throw new NotImplementedException();
        }
        public Task<List<FunctionDTO>> GetAllFunctionsAsync(FunctionDTO search = null)
        {
            throw new NotImplementedException();
        }
        public Task<FunctionDTO?> GetFunctionByIdAsync(int functionId)
        {
            throw new NotImplementedException();
        }
        public Task<string> UpdateFunctionAsync(FunctionDTO functionDto)
        {
            throw new NotImplementedException();
        }

        public async Task<List<SelectListItem>> GetRoleSL()
        {
            var data = await _context.Auth_Master_Role
                .Select(r => new SelectListItem
                {
                    Value = r.RoleId.ToString(),
                    Text = r.RoleName
                })
                .ToListAsync();
            return data;
        }

        public async Task<List<SelectListItem>> GetFunctionSL()
        {
            var data = await _context.Auth_Master_Function
                .Select(f => new SelectListItem
                {
                    Value = f.Id.ToString(),
                    Text = f.EnName
                })
                .ToListAsync();
            return data;
        }

        public async Task<RoleFuncPmsDTO?> GetRoleFuncPmsByIdAsync(int Id)
        {
            var data = await _context.Auth_Mapping_Role_Func_Pms
                .Include(i=>i.Role)
                .Include(i=>i.Function)
                .Include(i=>i.Pms)
                .Where(i => i.Id == Id).FirstOrDefaultAsync();
            var result = _map.Map<RoleFuncPmsDTO>(data);
            return result;
        }
        #endregion ----------------------------------------
    }
}
