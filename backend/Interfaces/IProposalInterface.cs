using backend.Dto;
using backend.Repositories;

namespace backend.Interfaces;

public interface IProposalInterface
{
    public Task<ErrorCodes> AddTestProposal(ProposalInDto dto);
    public Task<Tuple<List<ProposalAdminOutDto>, ErrorCodes>> ReturnAllProposals();
    public Task<Tuple<List<ProposalUserOutDto>, ErrorCodes>> ReturnUsersProposals();
    public Task<ErrorCodes> UserAnswersTheProposal(UserChangesStatusProposalDto dto);
    public Task<ErrorCodes> AdminChangesStatusTheProposal(AdminChangesStatusProposalDto dto);
}