using AutoMapper;
using Dynamiq.Application.DTOs.AccountDTOs;
using Dynamiq.Application.Interfaces.Repositories;
using Dynamiq.Application.Queries.Users.Queries;
using MediatR;

namespace Dynamiq.Application.Queries.Users.Handlers
{
    public class GetUserByIdHandler : IRequestHandler<GetUserByIdQuery, UserDto>
    {
        private readonly IUserRepo _userRepo;
        private readonly IMapper _mapper;

        public GetUserByIdHandler(IUserRepo userRepo, IMapper mapper)
        {
            _userRepo = userRepo;
            _mapper = mapper;
        }

        public async Task<UserDto> Handle(GetUserByIdQuery request, CancellationToken ct)
        {
            var user = await _userRepo.GetByIdAsync(request.Id, ct);

            if (user == null)
                throw new KeyNotFoundException($"User with Id {request.Id} not found");

            return _mapper.Map<UserDto>(user);
        }
    }
}
