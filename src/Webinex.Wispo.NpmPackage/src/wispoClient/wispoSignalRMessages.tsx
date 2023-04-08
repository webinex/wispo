export const WISPO_SIGNALR_MESSAGES = {
  /**
   * Sent when new notifications created.
   * args[0] - count
   * args[1] - ids
   * args[2] - notifications
   */
  New: 'wispo://NEW',

  /**
   * Sent when notifications marked as read.
   * args[0] - count
   * args[1] - ids
   */
  Read: 'wispo://READ',
} as const;
