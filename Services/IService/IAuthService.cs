using MESWebDev.Models.Master;
using MESWebDev.Models.Master.DTO;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace MESWebDev.Services.IService
{
    public interface IAuthService
    {
        //------ User -------------
        Task<List<UserDTO>> GetAllUsersAsync(UserDTO? search = null);
        Task<UserDTO?> GetUserByUsernameAsync(string username);
        Task<string> CreateUserAsync(UserDTO userDto);
        Task<string> UpdateUserAsync(UserDTO userDto);
        Task<bool> DeleteUserAsync(string username);

        //------ Role -------------
        Task<List<RoleModel>> GetAllRolesAsync(RoleDTO? search = null);
        Task<List<SelectListItem>> GetRoleSL();
        Task<RoleDTO?> GetRoleByIdAsync(int roleId);
        Task<string> CreateRoleAsync(RoleDTO roleDto);
        Task<string> UpdateRoleAsync(RoleDTO roleDto);
        Task<bool> DeleteRoleAsync(int roleId);

        //------ Function -------------
        Task<List<FunctionDTO>> GetAllFunctionsAsync(FunctionDTO? search = null);
        Task<List<SelectListItem>> GetFunctionSL();
        Task<FunctionDTO?> GetFunctionByIdAsync(int functionId);
        Task<string> CreateFunctionAsync(FunctionDTO functionDto);
        Task<string> UpdateFunctionAsync(FunctionDTO functionDto);
        Task<bool> DeleteFunctionAsync(int functionId);

        //------ Role Func Pms -------------
        Task<List<RoleFuncPmsDTO>> GetAllRoleFuncPmsAsync(RoleFuncPmsDTO? search = null);
        Task<RoleFuncPmsDTO?> GetRoleFuncPmsByIdAsync(int Id);

        Task<string> CreateRoleFuncPmsAsync(RoleFuncPmsDTO roleFuncPmsDto);
        Task<bool> DeleteRoleFuncPmsAsync(int id);
        Task<string> UpdateRoleFuncPmsAsync(RoleFuncPmsDTO roleFuncPmsDto);

        //------ Permission -----------
        //Task<List<SelectListItem>> GetPmsSL();



    }
}
