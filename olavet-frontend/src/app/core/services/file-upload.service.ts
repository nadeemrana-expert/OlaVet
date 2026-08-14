import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';
import { FileUploadResult } from '../models';

@Injectable({ providedIn: 'root' })
export class FileUploadService {
  private readonly apiUrl = `${environment.apiUrl}/fileupload`;

  constructor(private http: HttpClient) {}

  upload(file: File, subfolder?: string): Observable<FileUploadResult> {
    const formData = new FormData();
    formData.append('file', file, file.name);
    let url = this.apiUrl;
    if (subfolder) {
      url += `?subfolder=${encodeURIComponent(subfolder)}`;
    }
    return this.http.post<FileUploadResult>(url, formData);
  }

  delete(fileName: string): Observable<{ message: string }> {
    return this.http.delete<{ message: string }>(`${this.apiUrl}/${fileName}`);
  }
}
