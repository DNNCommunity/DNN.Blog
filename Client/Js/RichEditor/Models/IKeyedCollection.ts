export interface IKeyedCollection<T> {
  ContainsKey(key: string): boolean;
  Add(key: string, value: T): void;
  Item(key: string): T;
}

export class KeyedCollection<T> implements IKeyedCollection<T> {
  private items: Record<string, T> = {};

  ContainsKey(key: string): boolean {
    return Object.prototype.hasOwnProperty.call(this.items, key);
  }

  Add(key: string, value: T): void {
    this.items[key] = value;
  }

  Item(key: string): T {
    return this.items[key];
  }
}
