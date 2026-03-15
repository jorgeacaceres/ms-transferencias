using MediatR;
using ms_transferencias.Application.Commands.CreateTransfer;
using ms_transferencias.Application.Common.Models;
using ms_transferencias.Application.Queries.GetTransferById;

namespace ms_transferencias.Endpoints;

public static class TransferEnpoints
{
    public static void MapEndpoints(this IEndpointRouteBuilder routes)
    {
        var group = routes.MapGroup("/api");

        group.MapGet("/payments/{externalOperationId}", GetTransferStatusAsync)
             .WithName("GetTransferStatus")
             .WithOpenApi()
             .WithTags("Payments")
             .Produces<GetTransferByIdResponse>(StatusCodes.Status200OK)
             .Produces(StatusCodes.Status204NoContent)
             .Produces<ErrorResponse>(StatusCodes.Status400BadRequest)
             .Produces<ErrorResponse>(StatusCodes.Status500InternalServerError)
;

        group.MapPost("/payments", CreateTransfer)
             .WithName("CreateTransfer")
             .WithOpenApi()
             .WithTags("Payments")
             .Produces<CreateTransferResponse>(StatusCodes.Status200OK)
             .Produces<ErrorResponse>(StatusCodes.Status400BadRequest)
             .Produces<ErrorResponse>(StatusCodes.Status500InternalServerError);
    }

    /// <summary>
    /// Busca el estado de una transferencia utilizando su externalOperationId.
    /// </summary>
    private async static Task<IResult> GetTransferStatusAsync(string externalOperationId,
                                                              ISender mediator,
                                                              CancellationToken cls)
    {
        var result = await mediator.Send(new GetTransferByIdQuery { ExternalOperationId = externalOperationId }, cls);
        if (result.IsT0)
        {
            return result.AsT0 is not null ? Results.Ok(result.AsT0) : Results.NoContent();
        }
        else
        {
            return Results.BadRequest(result.AsT1);
        }
    }

    /// <summary>
    /// Crea una nueva transferencia. El título es obligatorio y no puede estar vacío.
    /// </summary>
    private async static Task<IResult> CreateTransfer(CreateTransferCommand command,
                                                      ISender mediator,
                                                      CancellationToken cls)
    {
        var result = await mediator.Send(command, cls);
        return result.IsT0 ?
            Results.Created($"/payments", result.AsT0) :
            Results.BadRequest(result.AsT1);
    }
}