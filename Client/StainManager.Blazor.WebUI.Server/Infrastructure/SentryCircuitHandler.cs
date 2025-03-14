using Microsoft.AspNetCore.Components.Server.Circuits;

namespace StainManager.Blazor.WebUI.Server.Infrastructure;

public class SentryCircuitHandler(
    IHub sentryHub,
    ILogger<SentryCircuitHandler> logger)
    : CircuitHandler
{
    private IDisposable? _circuitScope;

    public override Task OnCircuitOpenedAsync(
        Circuit circuit,
        CancellationToken cancellationToken)
    {
        var circuitId = circuit.Id;
        logger.LogInformation("Circuit opened: {CircuitId}", circuitId);
        
        _circuitScope = sentryHub.PushScope();
        
        sentryHub.ConfigureScope(scope =>
        {
            scope.SetTag("circuit_id", circuitId);
            scope.SetTag("circuit_type", "blazor_server");
        });
        
        return base.OnCircuitOpenedAsync(circuit, cancellationToken);
    }
    
    public override Task OnCircuitClosedAsync(
        Circuit circuit,
        CancellationToken cancellationToken)
    {
        logger.LogInformation("Circuit closed: {CircuitId}", circuit.Id);
        
        _circuitScope?.Dispose();
        
        return base.OnCircuitClosedAsync(circuit, cancellationToken);
    }

    public override Task OnConnectionDownAsync(
        Circuit circuit,
        CancellationToken cancellationToken)
    {
        logger.LogInformation("Connection down: {CircuitId}", circuit.Id);
        return base.OnConnectionDownAsync(circuit, cancellationToken);
    }

    public override Task OnConnectionUpAsync(
        Circuit circuit,
        CancellationToken cancellationToken)
    {
        logger.LogInformation("Connection restored: {CircuitId}", circuit.Id);
        return base.OnConnectionUpAsync(circuit, cancellationToken);
    }
}