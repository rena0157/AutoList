import { TestBed } from '@angular/core/testing';

import { AutoListApiService } from './auto-list-api.service';

describe('AutoListApiService', () => {
  beforeEach(() => TestBed.configureTestingModule({}));

  it('should be created', () => {
    const service: AutoListApiService = TestBed.get(AutoListApiService);
    expect(service).toBeTruthy();
  });
});
