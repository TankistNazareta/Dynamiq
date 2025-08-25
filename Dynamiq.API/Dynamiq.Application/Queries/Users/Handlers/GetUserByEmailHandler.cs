using AutoMapper;
using Dynamiq.Application.DTOs.AccountDTOs;
using Dynamiq.Application.Interfaces.Repositories;
using Dynamiq.Application.Queries.Users.Queries;
using MediatR;

namespace Dynamiq.Application.Queries.Users.Handlers
{
    public class GetUserByEmailHandler : IRequestHandler<GetUserByEmailQuery, UserDto>
    {
        private readonly IUserRepo _repo;
        private readonly IMapper _mapper;

        public GetUserByEmailHandler(IUserRepo userRepo, IMapper mapper)
        {
            _repo = userRepo;
            _mapper = mapper;
        }

        public async Task<UserDto> Handle(GetUserByEmailQuery request, CancellationToken cancellationToken)
        {
            var user = await _repo.GetByEmailAsync(request.Email, cancellationToken);

            if (user == null)
                throw new KeyNotFoundException("User wasn't found with email: " + request.Email);

            return _mapper.Map<UserDto>(user);
        }
    }
}
