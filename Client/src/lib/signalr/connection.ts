import * as signalR from '@microsoft/signalr';

class SignalRService {
    private connection: signalR.HubConnection | null = null;

    async connect(token: string): Promise<signalR.HubConnection> {
        if (this.connection) {
            return this.connection;
        }

        const apiUrl = process.env.NEXT_PUBLIC_API_URL || 'http://localhost:5000';

        this.connection = new signalR.HubConnectionBuilder()
            .withUrl(`${apiUrl}/hubs/battle`, {
                accessTokenFactory: () => token,
                transport: signalR.HttpTransportType.WebSockets
            })
            .withAutomaticReconnect()
            .configureLogging(signalR.LogLevel.Information)
            .build();

        await this.connection.start();
        console.warn('SignalR Connected');

        return this.connection;
    }

    async disconnect(): Promise<void> {
        if (this.connection) {
            await this.connection.stop();
            this.connection = null;
            console.warn('SignalR Disconnected');
        }
    }

    getConnection(): signalR.HubConnection | null {
        return this.connection;
    }
}

export const signalRService = new SignalRService();
