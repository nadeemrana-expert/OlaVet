import { Component, OnInit, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ReactiveFormsModule, FormBuilder, FormGroup, FormArray, Validators } from '@angular/forms';
import { ActivatedRoute, Router, RouterLink } from '@angular/router';
import { StoreService } from '../../../core/services/store.service';
import { NotificationService } from '../../../core/services/notification.service';

@Component({
  selector: 'app-order-form',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, RouterLink],
  template: `
    <div class="form-page">
      <a routerLink="/stores" class="back-link">← Back</a>
      <h1>Place Order</h1>
      <form [formGroup]="form" (ngSubmit)="submit()" class="form-card">
        <div class="form-group"><label>Pet Owner ID *</label><input formControlName="petOwnerId" type="number" /></div>
        <h3>Order Items</h3>
        @for (item of items.controls; track $index; let i = $index) {
          <div class="item-row" [formGroupName]="i">
            <input formControlName="medicineId" type="number" placeholder="Medicine ID" />
            <input formControlName="quantity" type="number" min="1" placeholder="Qty" />
            <button type="button" class="remove-btn" (click)="removeItem(i)">✕</button>
          </div>
        }
        <button type="button" class="add-btn" (click)="addItem()">+ Add Item</button>
        <div class="form-actions">
          <a routerLink="/stores" class="btn btn-cancel">Cancel</a>
          <button type="submit" class="btn btn-primary" [disabled]="loading">Place Order</button>
        </div>
      </form>
    </div>
  `,
  styles: [`
    .form-page { max-width:640px; margin:0 auto; }
    .back-link { display:inline-block; margin-bottom:0.75rem; color:#667eea; text-decoration:none; }
    h1 { margin:0 0 1.5rem; } h3 { margin:1rem 0 0.75rem; }
    .form-card { background:#fff; border-radius:12px; padding:2rem; box-shadow:0 2px 8px rgba(0,0,0,0.04); }
    .form-group { margin-bottom:1.25rem; display:flex; flex-direction:column;
      label { font-weight:500; margin-bottom:0.35rem; }
      input { padding:0.6rem 0.9rem; border:1px solid #e2e8f0; border-radius:8px; &:focus { border-color:#667eea; outline:none; } } }
    .item-row { display:flex; gap:0.75rem; margin-bottom:0.75rem; align-items:center;
      input { flex:1; padding:0.6rem 0.9rem; border:1px solid #e2e8f0; border-radius:8px; } }
    .remove-btn { background:none; border:none; color:#e53e3e; cursor:pointer; font-size:1.1rem; }
    .add-btn { background:none; border:1px dashed #667eea; color:#667eea; padding:0.5rem 1rem; border-radius:8px; cursor:pointer; font-size:0.9rem; width:100%; margin-bottom:1rem; }
    .form-actions { display:flex; justify-content:flex-end; gap:0.75rem; }
    .btn { padding:0.55rem 1.25rem; border:none; border-radius:8px; font-weight:600; cursor:pointer; text-decoration:none;
      &-primary { background:#667eea; color:#fff; &:disabled { opacity:0.6; } }
      &-cancel { background:#edf2f7; color:#555; } }
  `],
})
export class OrderFormComponent implements OnInit {
  private fb = inject(FormBuilder);
  private storeService = inject(StoreService);
  private route = inject(ActivatedRoute);
  private router = inject(Router);
  private notify = inject(NotificationService);
  form!: FormGroup;
  loading = false;

  get items(): FormArray { return this.form.get('items') as FormArray; }

  ngOnInit(): void {
    const storeId = Number(this.route.snapshot.paramMap.get('id'));
    this.form = this.fb.group({
      storeId: [storeId],
      petOwnerId: [null, Validators.required],
      items: this.fb.array([this.createItem()]),
    });
  }

  createItem(): FormGroup {
    return this.fb.group({ medicineId: [null, Validators.required], quantity: [1, [Validators.required, Validators.min(1)]] });
  }

  addItem(): void { this.items.push(this.createItem()); }
  removeItem(i: number): void { if (this.items.length > 1) this.items.removeAt(i); }

  submit(): void {
    if (this.form.invalid) { this.form.markAllAsTouched(); return; }
    this.loading = true;
    this.storeService.createOrder(this.form.value).subscribe({
      next: () => { this.notify.success('Order placed!'); this.router.navigate(['/stores']); },
      error: () => (this.loading = false),
    });
  }
}
