using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Abp.Application.Services.Dto;
using Abp.Authorization;
using Abp.AutoMapper;
using Abp.Domain.Repositories;
using Abp.Linq.Extensions;
using Enterprises.CMS.UserList.Dto;
using Enterprises.CMS.Users;
using Microsoft.AspNet.Identity;
using Webdiyer.WebControls.Mvc;

namespace Enterprises.CMS.UserList
{
    public class UserAppService : CMSAppServiceBase, IUserAppService
    {
        private readonly IRepository<User, long> _userRepository;
        private readonly IPermissionManager _permissionManager;

        public UserAppService(IRepository<User, long> userRepository, IPermissionManager permissionManager)
        {
            _userRepository = userRepository;
            _permissionManager = permissionManager;
        }

        public async Task ProhibitPermission(ProhibitPermissionInput input)
        {
            var user = await UserManager.GetUserByIdAsync(input.UserId);
            var permission = _permissionManager.GetPermission(input.PermissionName);

            await UserManager.ProhibitPermissionAsync(user, permission);
        }

      
        /// <summary>
        /// 获取用户列表
        /// </summary>
        /// <returns></returns>

        public async Task<ListResultDto<UserListDto>> GetUsers()
        {
            var users = await _userRepository.GetAllListAsync();
            return new ListResultDto<UserListDto>(users.MapTo<List<UserListDto>>());
        }

        /// <summary>
        /// 新增用户
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public void Create(CreateUserInput model)
        {
            var entity = model.MapTo<User>();
            entity.TenantId = 1;
            entity.IsEmailConfirmed = false;
            entity.IsMobileConfirmed = false;
            PasswordHasher passwordHasher=new PasswordHasher();
            entity.Password = passwordHasher.HashPassword("123456");
            entity.Surname = entity.Name;
            entity.Mobile = model.Mobile;
            entity.IsActive = true;
            var userid = _userRepository.InsertAndGetId(entity);
            Logger.Debug(userid.ToString);
        }

        /// <summary>
        /// 删除用户
        /// </summary>
        /// <param name="id"></param>
        public void Delete(long id)
        {
            _userRepository.Delete(id);
        }

        /// <summary>
        /// 获取用户修改信息
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public CreateUserInput GetSingle(long id)
        {
            var entity = _userRepository.Get(id);
            return entity.MapTo<CreateUserInput>();
        }

        /// <summary>
        /// 修改记录
        /// </summary>
        /// <param name="model"></param>
        public void Edit(CreateUserInput model)
        {
            _userRepository.Update(model.Id, (a) =>
            {
                a.Mobile = model.Mobile;
                a.EmailAddress = model.EmailAddress;
                a.Name = model.Name;
                a.UserName = model.UserName;
            });
        }

        /// <summary>
        /// 查找列表
        /// </summary>
        /// <param name="input"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        public PagedList<UserListDto> List(UsersSearchInput input, int id)
        {
            int pageSize = 10;
            var query = _userRepository.GetAll().WhereIf(!string.IsNullOrEmpty(input.Moble),t=>t.Mobile.Contains(input.Moble))
                    .WhereIf(!string.IsNullOrEmpty(input.Name),t => (t.UserName.Contains(input.Name.Trim()) || t.Name.Contains(input.Name.Trim())));

            int totalCount;
            var list = query.OrderBy(p => p.UserName).PageBy(id, pageSize, out totalCount);
            PagedList<UserListDto> result = new PagedList<UserListDto>(list.MapTo<List<UserListDto>>(), id, pageSize,
                totalCount);
            return result;
        }
    }
}