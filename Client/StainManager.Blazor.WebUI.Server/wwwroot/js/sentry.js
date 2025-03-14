// Extract log level mapping to avoid duplication
const levelMap = { debug: 0, info: 1, warning: 2, error: 3, fatal: 4 };

// Global config placeholder
window.sentryConfig = {};

// Track loading states
window.sentryState = {
    initialized: false
};

// Enable detailed logging for troubleshooting
const SENTRY_DEBUG = true;

// Debug logging helper
function sentryLog(message, obj = null) {
    if (!SENTRY_DEBUG) {
        return;
    }
    
    if (obj) {
        console.log(`[Sentry] ${message}`, obj);
        return;
    }

    console.log(`[Sentry] ${message}`);
}

window.initializeSentry = function(config) {
    sentryLog("Sentry config received from Blazor", config);
    window.sentryConfig = config;
    window.sentryState.configReceived = true;

    try {
        sentryLog("Initializing Sentry now");
        
        if (!window.Sentry) {
            sentryLog('ERROR: Sentry not available, cannot initialize');
            return;
        }
        
        sentryLog("Initializing Sentry with environment: " + window.sentryConfig.environment);

        Sentry.init({
            dsn: getSentryConfig("Dsn", "829081488547fd7772d5936b26a628f5"),
            environment: window.sentryConfig.environment,
            debug: getSentryConfig("Debug", "false").toLowerCase() === "true",
            sendDefaultPii: getSentryConfig("SendDefaultPii", "true").toLowerCase() === "true",
            tracesSampleRate: parseFloat(getSentryConfig("TracesSampleRate", "1.0")),
            replaysSessionSampleRate: parseFloat(getSentryConfig("ReplaysSessionSampleRate", "0.1")),
            replaysOnErrorSampleRate: parseFloat(getSentryConfig("ReplaysOnErrorSampleRate", "1.0")),
            integrations: [
                Sentry.browserTracingIntegration(),
                Sentry.breadcrumbsIntegration({
                    console: true, dom: true, fetch: true,
                    history: true, xhr: true
                }),
                Sentry.replayIntegration({
                    maskAllText: false, blockAllMedia: false
                })
            ],
            beforeBreadcrumb: (breadcrumb, hint) => {
                sentryLog("Evaluating breadcrumb", { level: breadcrumb.level, category: breadcrumb.category });
                return isAboveLogLevel(breadcrumb.level, "MinimumBreadcrumbLevel", "Debug") ? breadcrumb : null;
            },
            beforeSend: (event, hint) => {
                sentryLog("Evaluating event", { level: event.level });
                return isAboveLogLevel(event.level, "MinimumEventLevel", "Error") ? event : null;
            },
            // Set initial scope
            initialScope: {
                tags: {
                    environment: window.sentryConfig.environment,
                    app_type: "frontend"
                }
            }
        });

        window.sentryState.initialized = true;
        sentryLog("Sentry initialization successful");
    } catch (error) {
        sentryLog("Initialization failed with error", {
            message: error.message,
            stack: error.stack
        });
    }
}

// Helper function to read Sentry configuration
function getSentryConfig(key, defaultValue) {
    const configValue = window.sentryConfig.sentry
        ? window.sentryConfig.sentry[key] || defaultValue
        : defaultValue;

    sentryLog(`Config ${key} = ${configValue} (default: ${defaultValue})`);
    return configValue;
}

// Helper function to check log level threshold
function isAboveLogLevel(level, minLevelName, defaultLevel) {
    const minLevel = getSentryConfig(minLevelName, defaultLevel).toLowerCase();
    const currentLevelValue = levelMap[level || defaultLevel.toLowerCase()] || levelMap[defaultLevel.toLowerCase()];
    const minLevelValue = levelMap[minLevel] || levelMap[defaultLevel.toLowerCase()];

    const result = currentLevelValue >= minLevelValue;
    sentryLog(`Level check: ${level || '(undefined)'} >= ${minLevel}? ${result}`, {
        currentLevelValue,
        minLevelValue
    });

    return result;
}

// Exception handling helpers
window.sentryHelpers = window.sentryHelpers || {};
window.sentryHelpers.captureException = function (exceptionData) {
    sentryLog("Capturing exception from server", {
        name: exceptionData?.name,
        correlationId: exceptionData?.correlationId
    });

    if (!exceptionData) {
        sentryLog("Exception data is empty or undefined, cannot capture");
        return;
    }
    
    try {
        if (!window.Sentry || !window.sentryState.initialized) {
            sentryLog('Sentry not available or not initialized, cannot capture exception');
            return;
        }
        
        // Capture the exception with enhanced context
        sentryLog("Capturing exception in Sentry", exceptionData);
        const error = new Error(exceptionData.errorMessage || 'An error occurred');
        console.error(error);
        Sentry.captureException(error, {
            tags: {
                correlation_id: exceptionData.correlationId,
                server_exception_type: exceptionData.name,
                error_source: "client_from_server"
            },
            contexts: {
                "server_exception": {
                    display_message: exceptionData.message,
                    stack: exceptionData.stack
                }
            },
            level: "error"
        });
        
        sentryLog("Exception captured in Sentry");
    } catch (error) {
        sentryLog("Error capturing exception in Sentry", error)
    }
};