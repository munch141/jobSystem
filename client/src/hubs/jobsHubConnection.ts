import { HubConnectionBuilder, LogLevel } from '@microsoft/signalr'

const jobsHubConnection = new HubConnectionBuilder()
  .withUrl('https://localhost:5001/jobsHub')
  .configureLogging(LogLevel.Information)
  .build()

export default jobsHubConnection
