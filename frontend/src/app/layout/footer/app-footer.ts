import { Component, signal } from "@angular/core";
import { MatIcon } from "@angular/material/icon";
import { MatListItem, MatNavList } from "@angular/material/list";
import { DatePipe } from "@angular/common";

@Component({
  imports: [MatIcon, MatNavList, MatListItem, DatePipe],
  selector: 'app-footer',
  templateUrl: './app-footer.html',
  styleUrl: './app-footer.scss'
})
export class AppFooter {
  today = signal(new Date)
}