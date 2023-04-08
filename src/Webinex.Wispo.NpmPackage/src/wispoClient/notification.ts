export interface Notification {
  id: string;
  subject: string;
  body: string;
  read: boolean;
  createdAt: string;
  readById?: string;
  readAt?: string;
}
