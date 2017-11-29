export class ApiResult<T> {
  Code: number;
  Message: string;
  Data: T;
}
