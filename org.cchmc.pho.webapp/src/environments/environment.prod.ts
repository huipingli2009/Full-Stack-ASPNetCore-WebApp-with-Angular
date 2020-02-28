import { NgxLoggerLevel } from 'ngx-logger';
export const environment = {
  production: true,
  apiURL: '.',
  logLevel: NgxLoggerLevel.OFF,
  serverLogLevel: NgxLoggerLevel.ERROR,
  disableConsoleLogging: true
};
