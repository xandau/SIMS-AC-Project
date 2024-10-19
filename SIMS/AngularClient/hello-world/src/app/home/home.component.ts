import { Component } from '@angular/core';
import { CommonModule } from '@angular/common'; // Import CommonModule for *ngIf

@Component({
  selector: 'app-home',
  standalone: true,
  imports: [
    CommonModule // Add CommonModule to the imports
  ],
  templateUrl: './home.component.html',
  styleUrls: ['./home.component.css'],
})
export class HomeComponent {
  lightboxOpen = false;
  selectedImage: string | null = null;

  // Method to open the lightbox with the selected image
  openImage(imageUrl: string) {
    this.selectedImage = imageUrl;
    this.lightboxOpen = true;
  }

  // Method to close the lightbox
  closeImage() {
    this.lightboxOpen = false;
    this.selectedImage = null;
  }
}
