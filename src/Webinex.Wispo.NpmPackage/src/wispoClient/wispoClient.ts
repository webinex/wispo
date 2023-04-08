import axios, { AxiosInstance } from 'axios';
import * as SignalR from '@microsoft/signalr';
import { WISPO_SIGNALR_MESSAGES } from './wispoSignalRMessages';
import { WispoClientConfig, WispoHttpConfig, WispoSignalRConfig } from './wispoClientConfig';
import { NotificationListRequest } from './notificationListRequest';
import { NotificationListResponse } from './notificationListResponse';
import { guard } from '../util';

const DEFAULT_SIGNALR_CONFIG: WispoSignalRConfig = {
  hubUri: '/api/wispo/hub',
  headersFactory: () => Promise.resolve({}),
};

const DEFAULT_HTTP_CONFIG: WispoHttpConfig = {
  axios: axios.create({ baseURL: '/api/wispo' }),
};

/**
 * Type for SignalR message callback
 */
export type MessageSubscriber = (kind: string, payload: any[]) => any;

/**
 * Wispo client
 */
export class WispoClient {
  private _subscribers: MessageSubscriber[] = [];
  private _reconnectedSubscribers: Array<() => any> = [];

  /**
   * SignalR connection. Would be null until `connect` function call
   */
  public connection: SignalR.HubConnection = null!;

  /**
   * Client configuration, merged with default values
   */
  public config: WispoClientConfig;

  /**
   * Creates new instance of wispo client
   * @param config Config override, would be merged with default values
   */
  constructor(config?: WispoClientConfig) {
    config = config ?? {};
    const httpConfig = config.http ?? {};
    const signalRConfig = config?.signalR ?? {};

    this.config = {
      ...config,
      http: { ...DEFAULT_HTTP_CONFIG, ...httpConfig },
      signalR: { ...DEFAULT_SIGNALR_CONFIG, ...signalRConfig },
    };
  }

  /**
   * Initializes new connection with signalR hub and subscribes for messages
   */
  public connect = async () => {
    const headers = await this.config.signalR!.headersFactory!();

    this.connection = new SignalR.HubConnectionBuilder()
      .withUrl(this.config.signalR!.hubUri!, {
        accessTokenFactory: this.config.signalR!.accessTokenFactory
          ? this.config.signalR!.accessTokenFactory
          : undefined,
        headers,
      })
      .withAutomaticReconnect()
      .build();

    this.connection.onreconnected(() => this._reconnectedSubscribers.forEach((s) => s()));

    Object.values(WISPO_SIGNALR_MESSAGES).forEach((kind) => {
      this.connection.on(kind, (...args) => this.publishMessage(kind, args));
    });

    await this.connection.start();
  };

  private publishMessage = (kind: string, payload: any[]) => {
    this._subscribers.forEach((subscriber) => subscriber(kind, payload));
  };

  /**
   * Subsribes for SignalR messages
   * @param subscriber New message callback
   * @returns Dispose function
   */
  public subscribe = (subscriber: MessageSubscriber) => {
    guard.notNull(subscriber, 'subscriber');

    this._subscribers.push(subscriber);
    return () => this.unsubscribe(subscriber);
  };

  /**
   * Unsubscribes callback
   * @param subscriber Subscribe callback used when you called `subscribe`
   */
  public unsubscribe = (subscriber: MessageSubscriber) => {
    guard.notNull(subscriber, 'subscriber');

    const index = this._subscribers.indexOf(subscriber);
    this._subscribers.splice(index, 1);
  };

  /**
   * Subscribes SignalR reconnection.
   * Useful to fetch data when some of messages can be loosed after disconnection
   * @param subscriber subscribe callback
   * @returns Dispose function
   */
  public subscribeReconnected = (subscriber: () => any) => {
    guard.notNull(subscriber, 'subscriber');

    this._reconnectedSubscribers.push(subscriber);
    return () => this.unsubscribeReconnected(subscriber);
  };

  public unsubscribeReconnected = (subscriber: () => any) => {
    guard.notNull(subscriber, 'subscriber');

    this._reconnectedSubscribers.splice(this._reconnectedSubscribers.indexOf(subscriber), 1);
  };

  /**
   * Fetches unread notifications identifiers
   * @returns identifiers of unread notifications
   */
  public unreadIds = async (): Promise<string[]> => {
    const { data } = await this.axios.get('unread/ids');
    return data;
  };

  /**
   * Marks notifications as read
   * @param id Required. Notification identifiers which might be marked as read
   */
  public markRead = async (id: string[]): Promise<void> => {
    guard.notNull(id, 'id');
    if (id.length === 0) {
      return;
    }

    await this.axios.put('read', id);
  };

  /**
   * Marks all unread notificatiosn as read.
   */
  public markAllRead = async (): Promise<void> => {
    await this.axios.put('read-all');
  };

  /**
   * This method allows you to query exceed data for your needs
   * @param args query arguments
   * @returns data in accordance with `args` includes
   */
  public get = async (args: NotificationListRequest): Promise<NotificationListResponse> => {
    guard.notNull(args, 'args');

    const searchParams = this.convertToSearchParams(args);
    const uri = '?' + searchParams.toString();
    const result = await this.axios.get<NotificationListResponse>(uri);
    return result.data;
  };

  private get axios(): AxiosInstance {
    if (!this.config.http?.axios) throw new Error('Axios unspecified');
    return this.config.http.axios!;
  }

  private convertToSearchParams = (args: NotificationListRequest) => {
    const { filter, include, skip = null, take = null, sort = null } = args;

    const query = new URLSearchParams();
    filter && query.append('filter', JSON.stringify(filter));
    sort && query.append('sort', JSON.stringify(sort));
    include && query.append('include', include.join(','));
    skip && query.append('skip', skip.toString());
    take && query.append('take', take.toString());

    return query;
  };
}

const search = new URLSearchParams();
search.append('sort', 'srott');
search.append('filter', '123');
search.toString();
