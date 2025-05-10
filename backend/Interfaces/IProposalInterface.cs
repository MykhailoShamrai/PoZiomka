using backend.Dto;
using backend.Repositories;

namespace backend.Interfaces;

public interface IProposalInterface
{
    public Task<ErrorCodes> AddTestProposal(ProposalInDto dto);
    public Task<Tuple<List<ProposalAdminOutDto>, ErrorCodes>> ReturnAllProposals();
}