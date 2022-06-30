import { HubConnectionBuilder, LogLevel } from '@microsoft/signalr'

const jobStatusHubConnection = new HubConnectionBuilder()
  .withUrl('https://localhost:5001/jobStatusHub')
  .configureLogging(LogLevel.Information)
  .build()

export default jobStatusHubConnection
