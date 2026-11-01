import { Component } from "@angular/core";
import { MatButton } from "@angular/material/button";
import { MatDivider } from "@angular/material/divider";
import { MatFormFieldModule } from "@angular/material/form-field";
import { MatIcon } from "@angular/material/icon";
import { MatInputModule } from "@angular/material/input";

@Component({
  imports: [MatIcon, MatButton, MatFormFieldModule, MatInputModule, MatDivider],
  selector: 'app-header',
  templateUrl: './app-header.html',
  styleUrl: './app-header.scss'
})
export class AppHeader {

}