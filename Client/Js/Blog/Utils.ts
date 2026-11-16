export class Utils {
  public static UnNull(input: Date | undefined, defaultValue: Date): Date {
    if (input == undefined) {
      return defaultValue;
    } else {
      return input;
    }
  }
  public static Merge(input: any, mergeObject: any, property: string): void {
    if (input[property] == undefined) {
      input[property] = mergeObject[property];
    }
  }
  public static ArraySort(array: any[], sortProperty: string, sortDirection: string): any[] {
    var res = array;
    if (sortDirection == "asc") {
      res.sort((a, b) => {
        return a[sortProperty] > b[sortProperty] ? 1 : b[sortProperty] > a[sortProperty] ? -1 : 0;
      });
    } else {
      res.sort((a, b) => {
        return a[sortProperty] > b[sortProperty] ? -1 : b[sortProperty] > a[sortProperty] ? 1 : 0;
      });
    }
    return res;
  }
  public static ArrayContains(
    array: any[],
    propertyName: string | null,
    propertyValue: any
  ): boolean {
    if (propertyName) {
      for (var i = 0; i < array.length; i++) {
        if (array[i][propertyName] == propertyValue) {
          return true;
        }
      }
    } else {
      for (var i = 0; i < array.length; i++) {
        if (array[i] == propertyValue) {
          return true;
        }
      }
    }
    return false;
  }
  public static UnNullStr(input: any): string {
    if (input) return input;
    return "";
  }
  public static isInt(value: any): boolean {
    return (
      !isNaN(value) && parseInt(Number(value).toString()) == value && !isNaN(parseInt(value, 10))
    );
  }
  public static Round(value: number, decimals: number) {
    switch (decimals) {
      case 0:
        return Math.round(value);
      case 1:
        return Math.round(value * 10) / 10;
      case 2:
        return Math.round(value * 100) / 100;
      default:
        var m = Math.pow(10, decimals);
        return Math.round(value * m) / m;
    }
  }
}
