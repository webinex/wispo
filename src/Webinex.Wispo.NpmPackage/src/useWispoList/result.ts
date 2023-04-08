import { guard } from '../util';

/**
 * Wrapper around promise.
 * Allows you to control when throw or not promise rejection.
 * Promise has `.catch` call on creation and it would be handled by Wispo until
 * you call `.unwrap()`. In this case, exception would be thrown.
 */
export class Result<T> {
  constructor(private _promise: Promise<T>) {
    guard.notNull(_promise, 'promise');
  }

  public unwrap = (): Promise<T> => {
    return this._promise.then((x) => x);
  };
}
