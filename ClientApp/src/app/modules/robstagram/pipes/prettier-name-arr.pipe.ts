import { Pipe, PipeTransform } from '@angular/core';


/**
 * Print an array of username strings in a prettier format.
 *
 * Usage:
 *  string[] | prettierNameArr
 * Example:
 *  { [Rick, Morty, Homer, Bart, BoJack, Fry] | prettierNameArr }
 *  formats to: Rick, Morty and 4 others
 * @export
 * @class PrettierNameArrPipe
 * @implements {PipeTransform}
 */
@Pipe({
  name: 'prettierNameArr'
})
export class PrettierNameArrPipe implements PipeTransform {

  transform(nameArr: string[], args?: any): string {
    if (nameArr === null || nameArr.length === 0) {
      return null;
    }
    if (nameArr.length <= 1) {
      return nameArr.join();
    }
    if (nameArr.length <= 2) {
      return nameArr.join(' and ');
    }
    return nameArr.slice(0, 2).join(', ') + ` and ${nameArr.length - 2} others `;
  }
  
}
