import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';

import { FontAwesomeModule } from '@fortawesome/angular-fontawesome';
import { library } from '@fortawesome/fontawesome-svg-core';
import { faHome, faCamera, faUser, faHeart, faPlusSquare, faComment } from '@fortawesome/free-solid-svg-icons';

// Add an icon to the library for convenient access in other components
library.add(faHome, faCamera, faUser, faHeart, faPlusSquare, faComment);

@NgModule({
  imports: [
    CommonModule,
    FontAwesomeModule
  ],
  declarations: []
})
export class IconModule { }
