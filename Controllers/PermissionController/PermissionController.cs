﻿using CodeBE_COMP1640.Controllers.UserController;
using CodeBE_COMP1640.Models;
using CodeBE_COMP1640.Repositories;
using CodeBE_COMP1640.Services;
using CodeBE_COMP1640.Services.PermissionS;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace CodeBE_COMP1640.Controllers.PermissionController
{
    public class PermissionController : ControllerBase
    {
        private IPermissionService PermissionService;

        public PermissionController(IPermissionService PermissionService)
        {
            this.PermissionService = PermissionService;
        }

        [HttpPost, Route(PermissionRoute.ListPath), Authorize]
        public async Task<List<string>> ListPath([FromBody] UserDTO UserDTO)
        {
            User User = ConvertUserDTOToUserEntity(UserDTO);
            List<string> Paths = await PermissionService.ListPath(User);
            return Paths;
        }

        [HttpPost, Route(PermissionRoute.ListAllPath), Authorize]
        public async Task<List<string>> ListAllPath()
        {
            List<string> Paths = await PermissionService.ListAllPath();
            return Paths;
        }

        [HttpGet, Route(PermissionRoute.Init)]
        public async Task Init()
        {
            await PermissionService.Init();
        }

        [Route(PermissionRoute.ListPermission), HttpPost, Authorize]
        public async Task<ActionResult<List<PermissionDTO>>> ListPermission()
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (!await PermissionService.HasPermission(PermissionRoute.ListPermission, PermissionService.GetUserId()))
            {
                return Forbid();
            }

            List<Permission> Permissions = await PermissionService.ListPermission();
            List<PermissionDTO> PermissionDTOs = Permissions.Select(x => new PermissionDTO(x)).ToList();
            return PermissionDTOs;
        }

        [Route(PermissionRoute.ListPermissionByRole), HttpPost, Authorize]
        public async Task<ActionResult<List<PermissionDTO>>> ListPermissionByRole([FromBody] RoleDTO RoleDTO)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (!await PermissionService.HasPermission(PermissionRoute.ListPermissionByRole, PermissionService.GetUserId()))
            {
                return Forbid();
            }

            List<Permission> Permissions = await PermissionService.ListPermissionByRole(RoleDTO.RoleId);
            List<PermissionDTO> PermissionDTOs = Permissions.Select(x => new PermissionDTO(x)).ToList();
            return PermissionDTOs;
        }

        [Route(PermissionRoute.ListRole), HttpPost, Authorize]
        public async Task<ActionResult<List<RoleDTO>>> ListRole()
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (!await PermissionService.HasPermission(PermissionRoute.ListRole, PermissionService.GetUserId()))
            {
                return Forbid();
            }

            List<Role> Roles = await PermissionService.ListRole();
            List<RoleDTO> RoleDTOs = Roles.Select(x => new RoleDTO(x)).ToList();
            return RoleDTOs;
        }

        [Route(PermissionRoute.GetRole), HttpPost, Authorize]
        public async Task<ActionResult<RoleDTO>> GetRole([FromBody] RoleDTO RoleDTO)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (!await PermissionService.HasPermission(PermissionRoute.GetRole, PermissionService.GetUserId()))
            {
                return Forbid();
            }

            Role Role = ConvertRoleDTOToRoleEntity(RoleDTO);
            Role = await PermissionService.GetRole(Role.RoleId);
            RoleDTO = new RoleDTO(Role);
            return RoleDTO;
        }

        [Route(PermissionRoute.CreateRole), HttpPost, Authorize]
        public async Task<ActionResult<bool>> CreateRole([FromBody] RoleDTO RoleDTO)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (!await PermissionService.HasPermission(PermissionRoute.CreateRole, PermissionService.GetUserId()))
            {
                return Forbid();
            }

            Role Role = ConvertRoleDTOToRoleEntity(RoleDTO);
            bool isRegisterSuccess = await PermissionService.CreateRole(Role);
            if (isRegisterSuccess)
                return true;
            else
                return BadRequest("Create Role Fail");
        }

        [Route(PermissionRoute.UpdateRole), HttpPost, Authorize]
        public async Task<ActionResult<RoleDTO>> UpdateRole([FromBody] RoleDTO RoleDTO)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (!await PermissionService.HasPermission(PermissionRoute.UpdateRole, PermissionService.GetUserId()))
            {
                return Forbid();
            }

            Role Role = ConvertRoleDTOToRoleEntity(RoleDTO);
            Role = await PermissionService.UpdateRole(Role);
            RoleDTO = new RoleDTO(Role);
            if (RoleDTO != null)
                return RoleDTO;
            else
                return BadRequest(RoleDTO);
        }

        [Route(PermissionRoute.DeleteRole), HttpPost, Authorize]
        public async Task<ActionResult<RoleDTO>> DeleteRole([FromBody] RoleDTO RoleDTO)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (!await PermissionService.HasPermission(PermissionRoute.DeleteRole, PermissionService.GetUserId()))
            {
                return Forbid();
            }

            Role Role = ConvertRoleDTOToRoleEntity(RoleDTO);
            Role = await PermissionService.DeleteRole(Role);
            RoleDTO = new RoleDTO(Role);
            if (RoleDTO != null)
                return RoleDTO;
            else
                return BadRequest(RoleDTO);
        }

        private User ConvertUserDTOToUserEntity(UserDTO UserDTO)
        {
            User User = new User();
            User.UserId = UserDTO.UserId;
            User.Username = UserDTO.Username;
            User.Password = UserDTO.Password;
            User.Email = UserDTO.Email;
            User.Class = UserDTO.Class;
            User.Phone = UserDTO.Phone;
            User.Address = UserDTO.Address;
            User.DepartmentId = UserDTO.DepartmentId ?? 0;

            return User;
        }

        private Role ConvertRoleDTOToRoleEntity(RoleDTO RoleDTO)
        {
            Role role = new Role();
            role.RoleId = RoleDTO.RoleId;
            role.Description = RoleDTO.Description;
            role.Name = RoleDTO.Name;
            role.PermissonRoleMappings = RoleDTO.PermissonRoleMappings?.Select(x => new PermissonRoleMapping
            {
                Id = x.Id ?? 0,
                RoleId = x.RoleId,
                PermissionId = x.PermissionId
            }).ToList();
            return role;
        }
    }

}
