import { Component } from "@angular/core";
import { MatButton } from "@angular/material/button";
import { MatIcon } from "@angular/material/icon";

@Component({
  selector: 'catalog-hero',
  templateUrl: './catalog-hero.html',
  imports: [MatButton, MatIcon],
  styleUrls: ['../../../app.scss']
})
export class CatalogHero {
  
}