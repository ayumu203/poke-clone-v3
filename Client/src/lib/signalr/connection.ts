import * as signalR from '@microsoft/signalr';

class SignalRService {
    private hubConnection: signalR.HubConnection | null = null;

    async connect(token: string): Promise<signalR.HubConnection> {
        if (this.hubConnection) {
            return this.hubConnection;
        }

        const apiUrl = process.env._API_URL || 'http://localhost:5000';

        this.hubConnection = new signalR.HubConnectionBuilder()
            .withUrl(`${apiUrl}/hubs/battle`, {
                accessTokenFactory: () => token,
                transport: signalR.HttpTransportType.WebSockets
            })
            .withAutomaticReconnect()
            .configureLogging(signalR.LogLevel.Information)
            .build();

        await this.hubConnection.start();
        console.warn('SignalR Connected');

        return this.hubConnection;
    }

    async disconnect(): Promise<void> {
        if (this.hubConnection) {
            await this.hubConnection.stop();
            this.hubConnection = null;
            console.warn('SignalR Disconnected');
        }
    }

    getConnection(): signalR.HubConnection | null {
        return this.hubConnection;
    }
}

export const signalRService = new SignalRService();
