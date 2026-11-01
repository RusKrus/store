import { Component } from "@angular/core";
import { CatalogHero } from "./catalog-hero/catalog-hero";
import { CatalogList } from "./catalog-list/catalog-list";

@Component({
  selector: 'app-catalog',
  templateUrl: './app-catalog.html',
  imports: [CatalogHero, CatalogList]
})
export class AppCatalog {

}