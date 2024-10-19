import { ComponentFixture, TestBed } from '@angular/core/testing';

import { CreatedTicketsComponent } from './created-tickets.component';

describe('CreatedTicketsComponent', () => {
  let component: CreatedTicketsComponent;
  let fixture: ComponentFixture<CreatedTicketsComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [CreatedTicketsComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(CreatedTicketsComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
