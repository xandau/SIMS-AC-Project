import { Component, OnInit } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { MatTableModule } from '@angular/material/table';
import { MatPaginatorModule, PageEvent } from '@angular/material/paginator';
import { CommonModule } from '@angular/common';
import { CookieService } from 'ngx-cookie-service';

@Component({
  selector: 'app-logs',
  standalone: true,
  imports: [CommonModule, MatTableModule, MatPaginatorModule],
  templateUrl: './logs.component.html',
  styleUrls: ['./logs.component.css'],
})
export class LogsComponent implements OnInit {
  logs: any[] = []; // To hold all logs
  pagedLogs: any[] = []; // To hold logs for the current page
  isLoading = true;  // For the loading indicator
  errorMessage: string = '';  // For any errors
  isError = false;  // This flag is now added to track errors

  // Pagination properties
  pageSize = 10;  // Logs per page
  pageIndex = 0;  // Current page index
  totalLogs = 0;  // Total number of logs

  apiUrl = (window as any).__env?.WEB_API_URL;

  constructor(private http: HttpClient, private cookieService: CookieService) {}

  ngOnInit(): void {
    this.fetchLogs();
  }

  // Fetch logs from the API
  fetchLogs() {
    this.isLoading = true;
    this.isError = false;  // Reset error state before fetching logs

    const accessToken = this.cookieService.get('accessToken'); // Get accessToken from cookie

    const headers = new HttpHeaders({
      'Authorization': `Bearer ${accessToken}`,  // Include the Bearer token
      'Content-Type': 'application/json',
    });

    this.http.get(`${this.apiUrl}/log`, { headers }).subscribe({
      next: (response: any) => {
        this.logs = response;  // Assuming the response is an array of logs
        this.totalLogs = this.logs.length;  // Set the total number of logs
        this.updatePagedLogs();  // Update the logs for the current page
        this.isLoading = false;
      },
      error: (err) => {
        console.error('Error fetching logs', err);
        this.isError = true;  // Set error flag
        this.errorMessage = 'Error fetching logs. Please try again later.';
        this.isLoading = false;
      },
    });
  }

  // Update the paged logs based on the current page index and page size
  updatePagedLogs() {
    const startIndex = this.pageIndex * this.pageSize;
    const endIndex = startIndex + this.pageSize;
    this.pagedLogs = this.logs.slice(startIndex, endIndex);
  }

  // Handle page change
  onPageChange(event: PageEvent) {
    this.pageSize = event.pageSize;
    this.pageIndex = event.pageIndex;
    this.updatePagedLogs();
  }
}
