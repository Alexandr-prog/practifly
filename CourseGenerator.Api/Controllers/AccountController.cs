﻿using System;
using System.Threading.Tasks;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using AutoMapper;
using CourseGenerator.BLL.Interfaces;
using CourseGenerator.BLL.Infrastructure;
using CourseGenerator.BLL.DTO;
using Microsoft.AspNetCore.Authorization;
using CourseGenerator.Api.Infrastructure;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;
using System.Net.Mime;
using Swashbuckle.AspNetCore.Annotations;
using CourseGenerator.Api.Models;

namespace CourseGenerator.Api.Controllers
{
    /// <summary>
    /// Контролер для роботи з акаунтом
    /// </summary>
    [ApiController]
    [SwaggerTag("Контролер для роботи з акаунтом")]
    [Produces(MediaTypeNames.Application.Json, new string[] { MediaTypeNames.Application.Xml })]
    [Route("~/api/[controller]")]
    public class AccountController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly IUserManagementService _userManagementService;
        private readonly AuthOptions _authOptions;

        /// <summary>
        /// Конструктор
        /// </summary>
        /// <param name="mapper">Об'єкт мапера</param>
        /// <param name="userManagementService">Сервіс для керування користувачами</param>
        /// <param name="authOptions">Налаштування JWT токена</param>
        public AccountController(IMapper mapper, 
            IUserManagementService userManagementService,
            AuthOptions authOptions)
        {
            _mapper = mapper;
            _userManagementService = userManagementService;
            _authOptions = authOptions;
        }

        /// <summary>
        /// Cтворює аккаунт
        /// </summary>
        /// <param name="registrationModel">Дані для реєстрації</param>
        /// <returns>Статус-код</returns>
        /// <response code="201">Акаунт створено</response>
        /// <response code="400">Помилка при виконанні запиту</response>
        [HttpPost]
        [Consumes(MediaTypeNames.Application.Json, new string[] { MediaTypeNames.Application.Xml })]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> CreateAsync([FromBody] UserRegistrationModel registrationModel)
        {
            UserRegistrationDTO registrationDto = _mapper.Map<UserRegistrationDTO>(registrationModel);
            OperationInfo registrationResult = await _userManagementService
                .CreateAsync(registrationDto, "User");

            if (registrationResult.Succeeded)
                return StatusCode(StatusCodes.Status201Created);
            return BadRequest(registrationModel);
        }

        /// <summary>
        /// Генерує код підтвердження аутентифікації
        /// </summary>
        /// <returns>Код підтвердження аутентифікації або статус-код</returns>
        /// <response code="201">Код згенеровано</response>
        /// <response code="400">Помилка при виконанні запиту</response>
        /// <response code="401">Неавторизовано</response>
        /// <response code="403">Заборонено</response>
        [Obsolete]
        [Route("~/api/[controller]/confirm/phone")]
        [Authorize]
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> CreatePhoneConfirmAsync()
        {
            Random random = new Random();
            int code = random.Next(100000, 999999);

            string userName = HttpContext.User.Identity.Name;
            UserDetailsDTO userDetailsDto = await _userManagementService
                .GetDetailsByNameAsync(userName);

            if (userDetailsDto.PhoneNumber == null)
                return BadRequest("User don't have phone number");

            PhoneAuthDTO phoneAuthDto = new PhoneAuthDTO
            {
                PhoneNumber = userDetailsDto.PhoneNumber,
                Code = Convert.ToString(code)
            };

            OperationInfo creationResult = await _userManagementService
                .GetConfirmCodeAsync(phoneAuthDto);
            if (!creationResult.Succeeded)
                return BadRequest(creationResult.Message);

            return StatusCode(StatusCodes.Status201Created, phoneAuthDto.Code);
        }

        /// <summary>
        /// Аутентифікує користувача по логіну і паролю
        /// </summary>
        /// <param name="loginModel">Дані для входу</param>
        /// <returns>Повертає об'єкт, який містить токен та дані користувача</returns>
        /// <response code="200">Аутентифіковано</response>
        /// <response code="401">Неавторизовано</response>
        [Route("~/api/[controller]/[action]")]
        [HttpPost]
        [Consumes(MediaTypeNames.Application.Json, new string[] { MediaTypeNames.Application.Xml })]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> AuthenticateAsync([FromBody] UserLoginModel loginModel)
        {
            UserLoginDTO loginDto = _mapper.Map<UserLoginDTO>(loginModel);
            ClaimsIdentity identity = await _userManagementService.GetIdentityAsync(loginDto);
            if (identity == null)
                return Unauthorized(loginModel);

            AuthResponse authResponse = CreateAuthResponse(identity);
            return Ok(authResponse);
        }

        /// <summary>
        /// Аутентифікує користувача за номером та кодом
        /// </summary>
        /// <param name="phoneAuthModel">Номер телефону та код</param>
        /// <returns>Повертає об'єкт, який містить токен та дані користувача</returns>
        /// <response code="200">Аутентифіковано</response>
        /// <response code="401">Неавторизовано</response>
        [Obsolete]
        [Route("~/api/[controller]/authenticate/phone")]
        [HttpPost]
        [Consumes(MediaTypeNames.Application.Json, new string[] { MediaTypeNames.Application.Xml })]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> AuthenticatePhoneAsync([FromBody] PhoneAuthModel phoneAuthModel)
        {
            PhoneAuthDTO phoneAuthDto = _mapper.Map<PhoneAuthDTO>(phoneAuthModel);
            ClaimsIdentity identity = await _userManagementService.GetIdentityAsync(phoneAuthDto);
            if (identity == null)
                return Unauthorized($"Code \"{phoneAuthDto.Code}\" is invalid.");

            AuthResponse authResponse = CreateAuthResponse(identity);
            return Ok(authResponse);
        }

        /// <summary>
        /// Створює об'єкт з токеном та даними користувача (<see cref="AuthResponse"/>)
        /// </summary>
        /// <param name="identity">Створює посвідчення користувача</param>
        /// <returns>Об'єкт, що відправляється при успішній аутентифікації</returns>
        private AuthResponse CreateAuthResponse(ClaimsIdentity identity)
        {
            return new AuthResponse {
                access_token = CreateToken(identity),
                userId = identity.FindFirst(ClaimTypes.NameIdentifier).Value,
                userName = identity.Name,
                firstName = identity.FindFirst(ClaimTypes.GivenName).Value,
                lastName = identity.FindFirst(ClaimTypes.Surname).Value,
                langCode = identity.FindFirst(ClaimTypes.Locality).Value
            };
        }

        /// <summary>
        /// Створює токен для аутентифікації
        /// </summary>
        /// <param name="identity">Створює посвідчення користувача</param>
        /// <returns>JWT</returns>
        private string CreateToken(ClaimsIdentity identity)
        {
            var now = DateTime.Now;
            var token = new JwtSecurityToken(
                issuer: _authOptions.Issuer,
                audience: _authOptions.Audience,
                notBefore: now,
                claims: identity.Claims,
                expires: now.AddDays(_authOptions.Lifetime),
                signingCredentials: new SigningCredentials(_authOptions.GetSymmetricSecurityKey(),
                    SecurityAlgorithms.HmacSha256)
            );
            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}