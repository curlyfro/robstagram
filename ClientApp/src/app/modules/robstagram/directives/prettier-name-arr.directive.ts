import { Directive, ElementRef, Input, OnInit, Renderer2 } from '@angular/core';


/**
 * Print an array of username strings in a prettier format.
 * Adds links to individual usernames.
 *
 * Usage:
 *  string[] | prettierNameArr
 * Example:
 *  { [Rick, Morty, Homer, Bart, BoJack, Fry] | prettierNameArr }
 *  formats to: Rick, Morty and 4 others
 *  links to: user profile gallery pages
 *
 * @export
 * @class PrettierNameArrDirective
 * @implements {OnInit}
 */
@Directive({
  selector: '[appPrettierNames]'
})
export class PrettierNameArrDirective implements OnInit {
  // tslint:disable-next-line:no-input-rename
  @Input('appPrettierNames')
  nameArr: string[];

  constructor(private el: ElementRef, private renderer: Renderer2) { }

  ngOnInit() {
    let strong = document.createElement('strong');
    const nameArr = this.nameArr;
    const andSpan = document.createElement('span');
    andSpan.innerText = ' and ';
    const commaSpan = document.createElement('span');
    commaSpan.innerText = ', ';
    const classLink = 'text-dark';
    const baseUrl = '/robstagram/gallery/';

    if (nameArr === null || nameArr.length === 0) {
      this.el.nativeElement.innerText = '';
    } else if (nameArr.length <= 1) {
      // create and add link to span
      const link = document.createElement('a');
      link.href = `${baseUrl}${nameArr[0]}/`;
      link.className = classLink;
      this.renderer.appendChild(this.el.nativeElement, link);
      // create and add strong to link
      strong = document.createElement('strong');
      strong.innerText = nameArr.join();
      this.renderer.appendChild(link, strong);

      // this.el.nativeElement.innerText = nameArr.join();
    } else if (nameArr.length <= 2) {
      // first name link
      const first = document.createElement('a');
      first.href = `${baseUrl}${nameArr[0]}/`;
      first.className = classLink;
      this.renderer.appendChild(this.el.nativeElement, first);
      // create and add strong to first link
      strong = document.createElement('strong');
      strong.innerText = nameArr[0];
      this.renderer.appendChild(first, strong);
      // add and span
      this.renderer.appendChild(this.el.nativeElement, andSpan);
      // second name link
      const second = document.createElement('a');
      second.href = `${baseUrl}${nameArr[1]}/`;
      second.className = classLink;
      this.renderer.appendChild(this.el.nativeElement, second);
      // create and add strong to second link
      strong = document.createElement('strong');
      strong.innerText = nameArr[1];
      this.renderer.appendChild(second, strong);

      // this.el.nativeElement.innerText = nameArr.join(' and ');
    } else {
      // first name link
      const first = document.createElement('a');
      first.href = `${baseUrl}${nameArr[0]}/`;
      first.className = classLink;
      this.renderer.appendChild(this.el.nativeElement, first);
      // create and add strong to first link
      strong = document.createElement('strong');
      strong.innerText = nameArr[0];
      this.renderer.appendChild(first, strong);
      // add and span
      this.renderer.appendChild(this.el.nativeElement, commaSpan);
      // second name link
      const second = document.createElement('a');
      second.href = `${baseUrl}${nameArr[1]}/`;
      second.className = classLink;
      this.renderer.appendChild(this.el.nativeElement, second);
      // create and add strong to second link
      strong = document.createElement('strong');
      strong.innerText = nameArr[1];
      this.renderer.appendChild(second, strong);
      // add and span
      this.renderer.appendChild(this.el.nativeElement, andSpan);
      // add fake link
      const others = document.createElement('a');
      others.href = 'javascript:void(0)';
      others.className = classLink;
      this.renderer.appendChild(this.el.nativeElement, others);
      // create and add strong to others link
      strong = document.createElement('strong');
      strong.innerText = `${nameArr.length - 2} others `;
      this.renderer.appendChild(others, strong);

      // this.el.nativeElement.innerText = nameArr.slice(0, 2).join(', ') + ` and ${nameArr.length - 2} others `;
    }
  }
}
